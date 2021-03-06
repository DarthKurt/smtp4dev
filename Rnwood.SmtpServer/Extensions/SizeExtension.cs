﻿#region

using System;

#endregion

namespace Rnwood.SmtpServer.Extensions
{
    public class SizeExtension : IExtension
    {
        public IExtensionProcessor CreateExtensionProcessor(IConnection connection)
        {
            return new SizeExtensionProcessor(connection);
        }

        #region Nested type: SizeExtensionProcessor

        private class SizeExtensionProcessor : IExtensionProcessor, IParameterProcessor
        {
            public SizeExtensionProcessor(IConnection connection)
            {
                Connection = connection;
                Connection.MailVerb.FromSubVerb.ParameterProcessorMap.SetProcessor("SIZE", this);
            }

            public IConnection Connection { get; private set; }

            #region IParameterProcessor Members

            public void SetParameter(IConnection connection, string key, string value)
            {
                if (key.Equals("SIZE", StringComparison.OrdinalIgnoreCase))
                {
                    long messageSize;

                    if (long.TryParse(value, out messageSize) && messageSize > 0)
                    {
                        var maxMessageSize = Connection.Server.Behaviour.GetMaximumMessageSize(Connection);
                        connection.CurrentMessage.DeclaredMessageSize = messageSize;

                        if (maxMessageSize.HasValue && messageSize > maxMessageSize)
                        {
                            throw new SmtpServerException(
                                new SmtpResponse(StandardSmtpResponseCode.ExceededStorageAllocation,
                                                 "Message exceeds fixes size limit"));
                        }
                    }
                    else
                    {
                        throw new SmtpServerException(new SmtpResponse(StandardSmtpResponseCode.SyntaxErrorInCommandArguments, "Bad message size specified"));
                    }
                }
            }

            #endregion

            public string[] EhloKeywords
            {
                get
                {
                    var maxMessageSize = Connection.Server.Behaviour.GetMaximumMessageSize(Connection);

                    if (maxMessageSize.HasValue)
                    {
                        return new[] { string.Format("SIZE={0}", maxMessageSize.Value) };
                    }
                    else
                    {
                        return new[] { "SIZE" };
                    }
                }
            }
        }

        #endregion
    }
}