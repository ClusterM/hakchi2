using System;
using System.Net;

namespace mooftpserv
{
    /// <summary>
    /// Interface for a class managing user authentication and allowing connections.
    /// </summary>
    public interface IAuthHandler
    {
        /// <summary>
        /// Make a new instance for a new session with the given peer.
        /// Each FTP session uses a separate, cloned instance.
        /// </summary>
        IAuthHandler Clone(IPEndPoint peer);

        /// <summary>
        /// Check the given login. Note that the method can be called in three ways:
        /// - user and pass are null: anonymous authentication
        /// - pass is null: login only with username (e.g. "anonymous")
        /// - both are non-null: login with user and password
        /// </summary>
        /// <param name='user'>
        /// The username, or null.
        /// </param>
        /// <param name='pass'>
        /// The password, or null.
        /// </param>
        bool AllowLogin(string user, string pass);

        /// <summary>
        /// Check if a control connection from the peer should be allowed.
        /// </summary>
        bool AllowControlConnection();

        /// <summary>
        /// Check if the PORT command of the peer with the given
        /// target endpoint should be allowed.
        /// </summary>
        /// The argument given by the peer in the PORT command.
        /// </param>
        bool AllowActiveDataConnection(IPEndPoint target);
    }
}

