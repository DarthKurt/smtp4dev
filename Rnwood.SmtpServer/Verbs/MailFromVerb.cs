﻿#region

using System.Linq;
using System.Threading.Tasks;

#endregion

namespace Rnwood.SmtpServer.Verbs
{
    public class MailFromVerb : IVerb
    {
        private readonly ICurrentDateTimeProvider _currentDateTimeProvider;

        public MailFromVerb(ICurrentDateTimeProvider currentDateTimeProvider)
        {
            ParameterProcessorMap = new ParameterProcessorMap();
            _currentDateTimeProvider = currentDateTimeProvider;
        }

        public MailFromVerb() : this(new CurrentDateTimeProvider())
        {
        }

        public ParameterProcessorMap ParameterProcessorMap { get; private set; }

        public async Task ProcessAsync(IConnection connection, SmtpCommand command)
        {
            if (connection.CurrentMessage != null)
            {
                await connection.WriteResponseAsync(new SmtpResponse(StandardSmtpResponseCode.BadSequenceOfCommands,
                                                                   "You already told me who the message was from"));
                return;
            }

            if (command.ArgumentsText.Length == 0)
            {
                await connection.WriteResponseAsync(
                    new SmtpResponse(StandardSmtpResponseCode.SyntaxErrorInCommandArguments,
                                     "Must specify from address or <>"));
                return;
            }

            var argumentsParser = new ArgumentsParser(command.ArgumentsText);
            var arguments = argumentsParser.Arguments;

            var from = arguments.First();
            if (from.StartsWith("<"))
                from = from.Remove(0, 1);

            if (from.EndsWith(">"))
                from = from.Remove(from.Length - 1, 1);

            connection.Server.Behaviour.OnMessageStart(connection, from);
            connection.NewMessage();
            connection.CurrentMessage.ReceivedDate = _currentDateTimeProvider.GetCurrentDateTime();
            connection.CurrentMessage.From = from;

            try
            {
                await ParameterProcessorMap.ProcessAsync(connection, arguments.Skip(1).ToArray(), true);
                await connection.WriteResponseAsync(new SmtpResponse(StandardSmtpResponseCode.Ok, "New message started"));
            }
            catch
            {
                connection.AbortMessage();
                throw;
            }
        }
    }
}