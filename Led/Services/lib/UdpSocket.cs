using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Led.Services.lib
{
    class UdpSocket
    {
        Socket _Socket;

        private UdpClient _UdpServer;
        private IPEndPoint _ScanServerEndPoint;
        BackgroundWorker _ServerBackgroundWorker;

        public UdpSocket()
        {
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            _UdpServer = new UdpClient(Defines.ServerPort);
            _ScanServerEndPoint = new IPEndPoint(IPAddress.Any, Defines.ServerPort);
        }

        public void SendAnswer(IPAddress iPAddress, int port)
        {
            byte[] _answer = Encoding.ASCII.GetBytes(Defines.UdpBroadcastAnswer);
            byte[] _ip = Encoding.ASCII.GetBytes(ConnectivityService.GetLocalIPAddress().ToString());

            byte[] _sendBuffer = new byte[_answer.Length + _ip.Length];
            Buffer.BlockCopy(_answer, 0, _sendBuffer, 0, _answer.Length);
            Buffer.BlockCopy(_ip, 0, _sendBuffer, _answer.Length, _ip.Length);

            _SendMessage(_sendBuffer, iPAddress, port);
        }

        private void _SendMessage(byte[] sendbuffer, IPAddress iPAddress, int port)
        {
            try
            {
                IPEndPoint _iPEndPoint = new IPEndPoint(iPAddress, port);
                _Socket.SendTo(sendbuffer, _iPEndPoint);
            }
            catch (Exception e)
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

                            callback.BeginInvoke(ip, bytes, port, null, null);
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
}
