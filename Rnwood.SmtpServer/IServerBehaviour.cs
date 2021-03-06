#region

using Rnwood.SmtpServer.Extensions;
using Rnwood.SmtpServer.Extensions.Auth;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Rnwood.SmtpServer
{
    public interface IServerBehaviour
    {
        /// <summary>
        /// Gets domain name reported by the server to clients.
        /// </summary>
        /// <value>The domain name report by the server to clients.</value>
        string DomainName { get; }

        /// <summary>
        /// Gets the IP address on which to listen for connections.
        /// </summary>
        /// <value>The IP address.</value>
        IPAddress IpAddress { get; }

        /// <summary>
        /// Gets the TCP port number on which to listen for connections.
        /// </summary>
        /// <value>The TCP port number.</value>
        int PortNumber { get; }

        int MaximumNumberOfSequentialBadCommands { get; }

        /// <summary>
        /// Gets a value indicating whether to run in SSL mode.
        /// </summary>
        /// <value><c>true</c> if the server should run in SSL mode otherwise, <c>false</c>.</value>
        bool IsSslEnabled(IConnection connection);

        void OnMessageReceived(IConnection connection, IMessage message);

        void OnMessageRecipientAdding(IConnection connection, IMessageBuilder message, string recipient);

        /// <summary>
        /// Gets the maximum allowed size of the message for the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        long? GetMaximumMessageSize(IConnection connection);

        /// <summary>
        /// Gets the SSL certificate that should be used for the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        X509Certificate GetSslCertificate(IConnection connection);

        bool IsSessionLoggingEnabled(IConnection connection);

        /// <summary>
        /// Gets the extensions that should be enabled for the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        IEnumerable<IExtension> GetExtensions(IConnection connection);

        /// <summary>
        /// Called when a SMTP session is completed.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="session">The session.</param>
        void OnSessionCompleted(IConnection connection, ISession session);

        /// <summary>
        /// Called when a new SMTP session is started.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="session">The session.</param>
        void OnSessionStarted(IConnection connection, ISession session);

        /// <summary>
        /// Gets the receive timeout that should be used for the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        TimeSpan GetReceiveTimeout(IConnection connection);

        TimeSpan GetSendTimeout(IConnection connection);

        /// <summary>
        /// Validates the authentication request to determine if the supplied details
        /// are correct.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="authenticationRequest">The authentication request.</param>
        /// <returns></returns>
        Task<AuthenticationResult> ValidateAuthenticationCredentialsAsync(IConnection connection,
                                                           IAuthenticationCredentials authenticationRequest);

        /// <summary>
        /// Called when a new message is started in the specified session.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="from">From.</param>
        void OnMessageStart(IConnection connection, string from);

        IMessageBuilder OnCreateNewMessage(IConnection connection);

        /// <summary>
        /// Determines whether the speficied auth mechanism should be enabled for the specified connecton.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="authMechanism">The auth mechanism.</param>
        /// <returns>
        /// 	<c>true</c> if the specified auth mechanism should be enabled otherwise, <c>false</c>.
        /// </returns>
        bool IsAuthMechanismEnabled(IConnection connection, IAuthMechanism authMechanism);

        /// <summary>
        /// Called when a command received in the specified SMTP session.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="command">The command.</param>
        void OnCommandReceived(IConnection connection, SmtpCommand command);

        void OnMessageCompleted(IConnection connection);

        Encoding GetDefaultEncoding(IConnection connection);

        IEditableSession OnCreateNewSession(IConnection connection, IPAddress clientAddress, DateTime startDate);
    }
}