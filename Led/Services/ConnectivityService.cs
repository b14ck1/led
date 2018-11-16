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
using Led.Services.lib.TCP;

namespace Led.Services
{
    public class ConnectivityService
    {
        private lib.UdpSocket _UdpSocket;
        private lib.TCP.TcpServer _TcpServer;

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
            if (!_TcpServer.ClientMapping.ContainsKey(id))
                return;

            Model.LedEntity ledEntity;
            switch (tcpMessages)
            {
                case TcpMessages.Config:
                    ledEntity = _GetLedEntity(id);
                    if (ledEntity != null)
                        _TcpServer.ClientMapping[id].EnqueueMessage(new EntityMessage(TcpMessages.Config, lib.Parser.EntityConfig(ledEntity)));
                    break;
                case TcpMessages.Effects:
                    ledEntity = _GetLedEntity(id);
                    if (ledEntity != null)
                    {
                        if(!_TcpServer.ClientMapping[id].Client.ConfigSynchronized)
                            _TcpServer.ClientMapping[id].EnqueueMessage(new EntityMessage(TcpMessages.Config, lib.Parser.EntityConfig(ledEntity)));
                       
                        _TcpServer.ClientMapping[id].EnqueueMessage(new EntityMessage(TcpMessages.Config, lib.Parser.EntityEffect(ledEntity)));
                    }
                    break;
                case TcpMessages.Timestamp:
                    _TcpServer.ClientMapping[id].EnqueueMessage(new EntityMessage(TcpMessages.Timestamp, BitConverter.GetBytes(lib.HostNetworkConverter.Int32(frame))), true);                    
                    break;
                case TcpMessages.Play:
                    _TcpServer.ClientMapping[id].EnqueueMessage(new EntityMessage(TcpMessages.Play, null), true);
                    break;
                case TcpMessages.Pause:
                    _TcpServer.ClientMapping[id].EnqueueMessage(new EntityMessage(TcpMessages.Pause, null), true);
                    break;
                case TcpMessages.Preview:
                    break;
                case TcpMessages.Show:
                    _TcpServer.ClientMapping[id].EnqueueMessage(new EntityMessage(TcpMessages.Show, null));
                    break;
                case TcpMessages.Color:
                    break;
                default:
                    break;
            }
        }

        public ConnectivityService()
        {
            _UdpSocket = new lib.UdpSocket();            
            _TcpServer = new TcpServer(new lib.EntityHandlerProvider(), Defines.ServerPort);
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
