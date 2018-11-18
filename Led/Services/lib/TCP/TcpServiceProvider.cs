using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Services.lib.TCP
{
    /// <SUMMARY>
    /// Allows to provide the server with
    /// the actual code that is goint to service
    /// incoming connections.
    /// </SUMMARY>
    public abstract class TcpServiceProvider
    {
        public abstract string ID { get; }

        /// <SUMMARY>
        /// Provides a new instance of the object.
        /// </SUMMARY>
        public virtual object Clone()
        {
            throw new Exception("Derived clases" +
                      " must override Clone method.");
        }

        /// <SUMMARY>
        /// Gets executed when the server accepts a new connection.
        /// </SUMMARY>
        public abstract void OnAcceptConnection(ConnectionState state);

        /// <SUMMARY>
        /// Gets executed when the server detects incoming data.
        /// This method is called only if
        /// OnAcceptConnection has already finished.
        /// </SUMMARY>
        public abstract void OnReceiveData(ConnectionState state);

        /// <SUMMARY>
        /// Gets executed when the server needs to shutdown the connection.
        /// </SUMMARY>
        public abstract void OnDropConnection(ConnectionState state);
    }
}
