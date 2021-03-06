﻿using Moq;
using Rnwood.SmtpServer.Verbs;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Rnwood.SmtpServer.Tests
{
    public class Mocks
    {
        public Mocks()
        {
            Connection = new Mock<IConnection>();
            ConnectionChannel = new Mock<IConnectionChannel>();
            Session = new Mock<IEditableSession>();
            Server = new Mock<IServer>();
            ServerBehaviour = new Mock<IServerBehaviour>();
            MessageBuilder = new Mock<IMessageBuilder>();
            VerbMap = new Mock<IVerbMap>();

            ServerBehaviour.Setup(
                sb => sb.OnCreateNewSession(It.IsAny<IConnection>(), It.IsAny<IPAddress>(), It.IsAny<DateTime>())).
                Returns(Session.Object);
            ServerBehaviour.Setup(sb => sb.OnCreateNewMessage(It.IsAny<IConnection>())).Returns(MessageBuilder.Object);

            Connection.SetupGet(c => c.Session).Returns(Session.Object);
            Connection.SetupGet(c => c.Server).Returns(Server.Object);
            Connection.SetupGet(c => c.ReaderEncoding).Returns(new AsciiSevenBitTruncatingEncoding());
            Connection.Setup(s => s.CloseConnectionAsync()).Returns(() => ConnectionChannel.Object.CloseAync());

            Server.SetupGet(s => s.Behaviour).Returns(ServerBehaviour.Object);

            var isConnected = true;
            ConnectionChannel.Setup(s => s.IsConnected).Returns(() => isConnected);
            ConnectionChannel.Setup(s => s.CloseAync()).Returns(() => Task.Run(() => isConnected = false));
            ConnectionChannel.Setup(s => s.ClientIpAddress).Returns(IPAddress.Loopback);
        }

        public Mock<IConnection> Connection { get; private set; }
        public Mock<IConnectionChannel> ConnectionChannel { get; private set; }
        public Mock<IEditableSession> Session { get; private set; }
        public Mock<IServer> Server { get; private set; }
        public Mock<IServerBehaviour> ServerBehaviour { get; private set; }
        public Mock<IMessageBuilder> MessageBuilder { get; private set; }

        public Mock<IVerbMap> VerbMap { get; private set; }

        public void VerifyWriteResponseAsync(StandardSmtpResponseCode responseCode)
        {
            Connection.Verify(c => c.WriteResponseAsync(It.Is<SmtpResponse>(r => r.Code == (int)responseCode)));
        }
    }
}