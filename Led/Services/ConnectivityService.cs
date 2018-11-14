using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

        public void SendMessage(TcpMessages tcpMessages, string id, int frame = 0)
        {
            Model.LedEntity ledEntity;
            switch (tcpMessages)
            {
                case TcpMessages.Config:
                    ledEntity = _GetLedEntity(id);
                    if (ledEntity != null)
                    {
                        byte[] data = lib.Parser.EntityConfig(ledEntity);
                        _TcpServer.SendData(TcpMessages.Config, data, id);
                    }
                    break;
                case TcpMessages.Effects:
                    ledEntity = _GetLedEntity(id);
                    if (ledEntity != null)
                    {
                        if(!_TcpServer.ClientMapping[id].ConfigSynchronized)                        
                            _TcpServer.SendData(TcpMessages.Config, lib.Parser.EntityConfig(ledEntity), id);
                        
                        byte[] data = lib.Parser.EntityEffect(ledEntity);
                        _TcpServer.SendData(TcpMessages.Effects, data, id);
                    }
                    break;
                case TcpMessages.Timestamp:
                    _TcpServer.SendData(TcpMessages.Timestamp, BitConverter.GetBytes(lib.HostNetworkConverter.Int32(frame)), id);
                    break;
                case TcpMessages.Play:
                    _TcpServer.SendData(TcpMessages.Play, null, id);
                    break;
                case TcpMessages.Pause:
                    _TcpServer.SendData(TcpMessages.Pause, null, id);
                    break;
                case TcpMessages.Preview:
                    break;
                case TcpMessages.Show:
                    _TcpServer.SendData(TcpMessages.Show, null, id);
                    break;
                case TcpMessages.Color:
                    break;
                case TcpMessages.Resend:
                    break;
                case TcpMessages.Ready:
                    break;
                case TcpMessages.Heartbeat:
                    break;
                default:
                    break;
            }
        }

        public ConnectivityService()
        {
            _UdpSocket = new lib.UdpSocket();            
            _TcpServer = new lib.TCP.TcpServer(new lib.EntityHandlerProvider(), Defines.ServerPort);
        }

        private Model.LedEntity _GetLedEntity(string id)
        {
            foreach(var x in App.Instance.MainWindowVM.LedEntities)
            {
                if (x.LedEntity.ClientID.Equals(id))
                    return x.LedEntity;
            }
            return null;
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

        /// <summary>
        /// Searches for connected interfaces and excludes:
        /// - not connected
        /// - loopback
        /// - virtual
        /// - ipv6
        /// </summary>
        /// <returns>First found UnicastAddress</returns>
        public static IPAddress GetLocalIPAddress()
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {
                    string description = adapter.Description;
                    if (!description.Contains("Loopback") && !description.Contains("loopback") && !description.Contains("virtual"))
                    {
                        foreach (var address in adapter.GetIPProperties().UnicastAddresses)
                        {
                            if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                                return address.Address;
                        }
                    }
                }
            }
            throw new Exception("No suitable network adapters with an IPv4 address in the system!");
        }
    }
}
