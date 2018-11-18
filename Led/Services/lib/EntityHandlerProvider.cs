using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Led.Services.lib.TCP;

namespace Led.Services.lib
{
    public class EntityHandlerProvider : TCP.TcpServiceProvider
    {
        private bool _MessageIdentified;
        private TcpMessages _IncomingMessage;
        private Int32 _DataLength;
        private Int32 _DataReceived;
        private byte[] _Data;
        private static EntityMessage _HeartbeatAnswer;
        private static EntityMessage _IDRequest;
        public ViewModels.NetworkClientVM Client;

        private string _ID;
        public override string ID { get => _ID; }

        public override object Clone()
        {
            EntityHandlerProvider res = new EntityHandlerProvider();
            return res;
        }

        public override void OnAcceptConnection(ConnectionState state)
        {
            _MessageIdentified = false;
            state.Write(_IDRequest.Data, 0, _IDRequest.Data.Length);
        }

        /*
         *  MESSAGE FORMAT:
         *  2 byte Secret       uint16
         *  2 byte Message_ID   uint16
         *  4 byte Data_Length  int32
         *  * byte Data 
         * 
         */
        public override void OnReceiveData(ConnectionState state)
        {
            byte[] buffer = new byte[1024];
            while (state.AvailableData > 0)
            {
                int readBytes = state.Read(buffer, 0, 1024);
                if (readBytes > 0)
                {                    
                    if (!_MessageIdentified)
                    {
                        short _secret = HostNetworkConverter.Int16(BitConverter.ToInt16(buffer, 0));
                        if (_secret != 42)
                            state.EndConnection();

                        _IncomingMessage = (TcpMessages)HostNetworkConverter.Int16(BitConverter.ToInt16(buffer, 2));
                        _DataLength = HostNetworkConverter.Int32(BitConverter.ToInt32(buffer, 4));
                        _Data = new byte[_DataLength];

                        Int32 _dataToCopy = (Int32)(_DataLength - _DataReceived >= readBytes ? readBytes : _DataLength - _DataReceived);
                        Buffer.BlockCopy(buffer, 8, _Data, _DataReceived, _dataToCopy);
                        _DataReceived += _dataToCopy;

                        _MessageIdentified = true;
                    }
                    else
                    {
                        Int32 _dataToCopy = (Int32)(_DataLength - _DataReceived >= readBytes ? readBytes : _DataLength - _DataReceived);
                        Buffer.BlockCopy(buffer, 0, _Data, _DataReceived, _dataToCopy);
                        _DataReceived += _dataToCopy;
                    }

                    if (_DataReceived >= _DataLength)                    
                        _HandleData();               
                }
                else state.EndConnection();
                //If read fails then close connection
            }
        }

        public override void OnDropConnection(ConnectionState state)
        {
            App.Instance.MediatorService.BroadcastMessage(MediatorMessages.TcpServer_NetworkClientDropped, this, Client);
        }

        private void _SendMessage (EntityMessage entityMessage)
        {
            App.Instance.ConnectivityService.SendMessage(entityMessage, ID);
            Client.LastMessageSent = entityMessage;
        }
        

        static EntityHandlerProvider()
        {
            _IDRequest = new EntityMessage(TcpMessages.ID, null);
            _HeartbeatAnswer = new EntityMessage(TcpMessages.Heartbeat, null);
        }

        public EntityHandlerProvider()
        {
            Client = new ViewModels.NetworkClientVM();
        }

        private void _HandleData()
        {
            switch (_IncomingMessage)
            {
                case TcpMessages.ID:
                    _ID = Encoding.ASCII.GetString(_Data);
                    Console.WriteLine("Received ID from client: {0}", ID);
                    Client.ID = ID;                    

                    App.Instance.MediatorService.BroadcastMessage(MediatorMessages.TcpServer_NetworkClientAdded, this, Client);
                    goto default;
                case TcpMessages.Heartbeat:
                    _SendMessage(_HeartbeatAnswer);
                    break;
                case TcpMessages.Ready:
                    Client.Ready = true;
                    goto default;
                default: 
                    Client.LastMessageReceived = _IncomingMessage;
                    break;
            }

            _MessageIdentified = false;
            _DataLength = 0;
            _DataReceived = 0;
        }
    }
}
