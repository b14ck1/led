using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Led.Services.lib.TCP
{
    public class TcpServer
    {
        private int _port;
        private Socket _listener;
        private TcpServiceProvider _provider;
        private List<ConnectionState> _connections;
        public ConcurrentDictionary<string, EntityHandlerProvider> ClientMapping;
        private int _maxConnections = 100;

        private AsyncCallback _ConnectionReady;
        private WaitCallback _AcceptConnection;
        private AsyncCallback _ReceivedDataReady;

        /// <SUMMARY>
        /// Initializes server. To start accepting
        /// connections call Start method.
        /// </SUMMARY>
        public TcpServer(TcpServiceProvider provider, int port)
        {
            _provider = provider;
            _provider.TcpServer = this;

            _port = port;
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _connections = new List<ConnectionState>();
            _ConnectionReady = new AsyncCallback(ConnectionReady_Handler);
            _AcceptConnection = new WaitCallback(AcceptConnection_Handler);
            _ReceivedDataReady = new AsyncCallback(ReceivedDataReady_Handler);

            ClientMapping = new ConcurrentDictionary<string, EntityHandlerProvider>();            
        }

        /// <SUMMARY>
        /// Start accepting connections.
        /// A false return value tell you that the port is not available.
        /// </SUMMARY>
        public bool Start()
        {
            try
            {
                _listener.Bind(new IPEndPoint(ConnectivityService.GetLocalIPAddress(), _port));
                _listener.Listen(100);
                _listener.BeginAccept(_ConnectionReady, null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <SUMMARY>
        /// Shutsdown the server
        /// </SUMMARY>
        public void Stop()
        {
            lock (this)
            {
                _listener.Close();
                _listener = null;
                //Close all active connections
                foreach (object obj in _connections)
                {
                    ConnectionState st = obj as ConnectionState;
                    DropConnection(st);
                }
                _connections.Clear();
            }
        }

        /// <SUMMARY>
        /// Callback function: A new connection is waiting.
        /// </SUMMARY>
        private void ConnectionReady_Handler(IAsyncResult ar)
        {
            lock (this)
            {
                if (_listener == null) return;
                Socket conn = _listener.EndAccept(ar);
                if (_connections.Count >= _maxConnections)
                {
                    //Max number of connections reached.
                    string msg = "SE001: Server busy";
                    conn.Send(Encoding.UTF8.GetBytes(msg), 0,
                              msg.Length, SocketFlags.None);
                    conn.Shutdown(SocketShutdown.Both);
                    conn.Close();
                }
                else
                {
                    //Start servicing a new connection
                    ConnectionState st = new ConnectionState();
                    st._conn = conn;
                    st._server = this;
                    st._provider = (TcpServiceProvider)_provider.Clone(st);
                    st._buffer = new byte[4];
                    _connections.Add(st);
                    //Queue the rest of the job to be executed latter
                    ThreadPool.QueueUserWorkItem(_AcceptConnection, st);
                }
                //Resume the listening callback loop
                _listener.BeginAccept(_ConnectionReady, null);
            }
        }


        /// <SUMMARY>
        /// Executes OnAcceptConnection method from the service provider.
        /// </SUMMARY>
        private void AcceptConnection_Handler(object state)
        {
            ConnectionState st = state as ConnectionState;
            try { st._provider.OnAcceptConnection(st); }
            catch
            {
                //report error in provider... Probably to the EventLog
            }
            //Starts the ReceiveData callback loop
            if (st._conn.Connected)
                st._conn.BeginReceive(st._buffer, 0, 0, SocketFlags.None,
                  _ReceivedDataReady, st);
        }


        /// <SUMMARY>
        /// Executes OnReceiveData method from the service provider.
        /// </SUMMARY>
        private void ReceivedDataReady_Handler(IAsyncResult ar)
        {
            try
            {
                ConnectionState st = ar.AsyncState as ConnectionState;
                st._conn.EndReceive(ar);
                //Im considering the following condition as a signal that the
                //remote host droped the connection.
                if (st._conn.Available == 0) DropConnection(st);
                else
                {
                    try { st._provider.OnReceiveData(st); }
                    catch
                    {
                        //report error in the provider
                    }
                    //Resume ReceivedData callback loop
                    if (st._conn.Connected)
                        st._conn.BeginReceive(st._buffer, 0, 0, SocketFlags.None,
                          _ReceivedDataReady, st);
                }

            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                DropConnection(ar.AsyncState as ConnectionState);
            }
        }

        /// <SUMMARY>
        /// Removes a connection from the list
        /// </SUMMARY>
        internal void DropConnection(ConnectionState st)
        {
            lock (this)
            {
                st._conn.Shutdown(SocketShutdown.Both);
                st._conn.Close();

                try { st._provider.OnDropConnection(st); }
                catch
                {
                    //some error in the provider
                }
                                
                if (_connections.Contains(st))
                    _connections.Remove(st);
            }
        }

        public int MaxConnections
        {
            get
            {
                return _maxConnections;
            }
            set
            {
                _maxConnections = value;
            }
        }

        public int CurrentConnections
        {
            get
            {
                lock (this) { return _connections.Count; }
            }
        }

        public void AddClientMapping(EntityHandlerProvider entityHandlerProvider)
        {
            if (!ClientMapping.ContainsKey(entityHandlerProvider.ID))
                ClientMapping.TryAdd(entityHandlerProvider.ID, entityHandlerProvider);            
        }

        public void RemoveClientMaping(EntityHandlerProvider entityHandlerProvider)
        {
            if (ClientMapping.ContainsKey(entityHandlerProvider.ID))
                ClientMapping.TryRemove(entityHandlerProvider.ID, out var ignore);
        }
    }
}
