using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Led.Services
{
    public class ConnectivityService
    {
        private lib.UdpSocket _UdpSocket;
        private lib.TCP.TcpServer _TcpServer;

        public List<string> ConnectedClients { get => _TcpServer.ConnectedClients; }

        public void StartServer()
        {
            _UdpSocket.StartListening(ClientBroadcasted);
            _TcpServer.Start();
        }

        public void StopServer()
        {
            _UdpSocket.StopListening();
            _TcpServer.Stop();
        }

        public void SendTimeStamp(long frame, string id)
        {
            _TcpServer.SendData(TcpMessages.Timestamp, BitConverter.GetBytes(frame), id);
        }

        public void SendEntityConfig(Model.LedEntity ledEntity, string id)
        {
            byte[] data = lib.Parser.EntityConfig(ledEntity);
            _TcpServer.SendData(TcpMessages.Config, data, id);
        }

        public void SendPlay(string id)
        {
            _TcpServer.SendData(TcpMessages.Play, null, id);
        }

        public void SendPause(string id)
        {
            _TcpServer.SendData(TcpMessages.Pause, null, id);
        }

        public void SendEntityEffects(Model.LedEntity ledEntity, string id)
        {
            byte[] data = lib.Parser.EntityEffect(ledEntity);
            _TcpServer.SendData(TcpMessages.RenderedEffects, data, id);
        }

        public void SendShow(string id)
        {
            _TcpServer.SendData(TcpMessages.Show, null, id);
        }

        public ConnectivityService()
        {
            _UdpSocket = new lib.UdpSocket();            
            _TcpServer = new lib.TCP.TcpServer(new lib.EntityHandlerProvider(), Defines.ServerPort);
        }

        private void ClientBroadcasted(IPAddress ip, byte[] data, int port)
        {
            Console.WriteLine("IP: {0}, Port: {1}, Data: {2}", ip, port, Encoding.ASCII.GetString(data));

            if (Encoding.ASCII.GetString(data).Equals(Defines.UdpBroadcastMessage))
            {
                Console.WriteLine("Sending Answer.");
                _UdpSocket.SendAnswer(ip, port);
            }
        }
    }
}
