using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Led.Services
{
    public class ConnectivityService
    {
        private UDP _UDP;
        private TCP _TCP;

        //private BackgroundWorker _ScanUDPBackgroundWorker;

        public List<EntityClient> UnbindedClients { get; }
        public Dictionary<ViewModels.LedEntityBaseVM, EntityClient> ClientMapping { get; }

        public void StartServer()
        {
            _UDP.StartListening(ClientResponded);
            //_ScanUDPBackgroundWorker = new BackgroundWorker();
            //_ScanUDPBackgroundWorker.DoWork += new DoWorkEventHandler(delegate (object o, DoWorkEventArgs e)
            //{
            //    BackgroundWorker _backgroundWorker = o as BackgroundWorker;
            //    try
            //    {
            //        while (!_backgroundWorker.CancellationPending)
            //        {
            //            _UDP.SendScanMessage();
            //            Thread.Sleep(1000);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        Console.WriteLine(e.ToString());
            //    }
            //});
            //_ScanUDPBackgroundWorker.RunWorkerAsync();
        }

        public void StopServer()
        {
            //_ScanUDPBackgroundWorker.CancelAsync();
            _UDP.StopListening();
        }

        public void SendTimeStamp(long frame, ViewModels.LedEntityBaseVM ledEntityBaseVM = null)
        {
            if (ledEntityBaseVM == null)
                ClientMapping.Values.ToList().ForEach(x => _UDP.SendTimeStamp(frame, x.IP, x.Port));
            else if (ClientMapping.ContainsKey(ledEntityBaseVM))
                _UDP.SendTimeStamp(frame, ClientMapping[ledEntityBaseVM].IP, ClientMapping[ledEntityBaseVM].Port);
        }

        public ConnectivityService()
        {
            _UDP = new UDP();
            _TCP = new TCP();
            
            ClientMapping = new Dictionary<ViewModels.LedEntityBaseVM, EntityClient>();
        }

        private void ClientResponded(IPAddress IP, byte[] data, int port)
        {
            EntityClient entityClient = new EntityClient(IP, Encoding.ASCII.GetString(data), port);
            if (!UnbindedClients.Contains(entityClient))
                UnbindedClients.Add(entityClient);

            Console.WriteLine("IP: {0}, Port: {1}, Data: {2}", IP, port, Encoding.ASCII.GetString(data));

            _UDP.SendTimeStamp(200, entityClient.IP, entityClient.Port);

            Thread.Sleep(1000);

            _UDP.SendTimeStamp(200, entityClient.IP, entityClient.Port);
            _UDP.SendTimeStamp(200, entityClient.IP, entityClient.Port);
        }

        class UDP
        {
            Socket _Socket;

            private UdpClient _UdpServer;
            private IPEndPoint _ScanServerEndPoint;
            BackgroundWorker _ServerBackgroundWorker;

            public UDP()
            {
                _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                _UdpServer = new UdpClient(Defines.UdpServerPort);
                _ScanServerEndPoint = new IPEndPoint(IPAddress.Any, Defines.UdpServerPort);
            }

            public void SendTimeStamp(long frame, IPAddress iPAddress, int port)
            {
                byte[] _prefix = Encoding.ASCII.GetBytes(Defines.TimestampPrefix);
                byte[] _time = BitConverter.GetBytes(frame);

                byte[] _sendBuffer = new byte[_prefix.Length + _time.Length];
                Buffer.BlockCopy(_prefix, 0, _sendBuffer, 0, _prefix.Length);
                Buffer.BlockCopy(_time, 0, _sendBuffer, _prefix.Length, _time.Length);

                _SendMessage(_sendBuffer, iPAddress, port);
            }

            private void _SendMessage(byte[] sendbuffer, IPAddress iPAddress, int port)
            {
                try
                {
                    IPEndPoint _iPEndPoint = new IPEndPoint(iPAddress, port);
                    _Socket.SendTo(sendbuffer, _iPEndPoint);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            public void StartListening(Action<IPAddress, byte[], int> callback)
            {
                _ServerBackgroundWorker = new BackgroundWorker();
                _ServerBackgroundWorker.DoWork += new DoWorkEventHandler(
                    delegate (object o, DoWorkEventArgs e)
                    {
                        BackgroundWorker _backgroundWorker = o as BackgroundWorker;
                        try
                        {
                            while (!_backgroundWorker.CancellationPending)
                            {
                                byte[] bytes = _UdpServer.Receive(ref _ScanServerEndPoint);

                                IPAddress ip = _ScanServerEndPoint.Address;                                
                                int port = _ScanServerEndPoint.Port;

                                callback(ip, bytes, port);
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    });

                _ServerBackgroundWorker.RunWorkerAsync();
            }

            public void StopListening()
            {
                _ServerBackgroundWorker.CancelAsync();
            }
        }

        class TCP
        {

        }

        public class EntityClient
        {
            public IPAddress IP { get; }

            public string ID { get; }

            public int Port { get; }

            public EntityClient(IPAddress ip, string id, int port)
            {
                IP = ip;
                ID = id;
                Port = port;
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                //       
                // See the full list of guidelines at
                //   http://go.microsoft.com/fwlink/?LinkID=85237  
                // and also the guidance for operator== at
                //   http://go.microsoft.com/fwlink/?LinkId=85238
                //

                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                                
                if ((obj as EntityClient).ID.Equals(ID))
                    return true;
                else
                    return false;
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {                                
                return ID.GetHashCode();
            }
        }
    }
}
