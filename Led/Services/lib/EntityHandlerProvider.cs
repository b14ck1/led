using System;
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
        private byte[] _HeartbeatAnswer;
        private byte[] _IDRequest;
        private ViewModels.NetworkClientVM _Client;

        public override TcpServer TcpServer { get; set; }

        public string ID { get; private set; }        

        public override object Clone()
        {
            return new EntityHandlerProvider(TcpServer);
        }

        public override void OnAcceptConnection(ConnectionState state)
        {
            _MessageIdentified = false;
            if (!state.Write(_IDRequest, 0, _IDRequest.Length))
                state.EndConnection();
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
                        _HandleData(state);               
                }
                else state.EndConnection();
                //If read fails then close connection
            }
        }

        public override void OnDropConnection(ConnectionState state)
        {
            App.Instance.MediatorService.BroadcastMessage(MediatorMessages.TcpServer_NetworkClientDropped, this, _Client);
        }

        public EntityHandlerProvider()
        {
            _IDRequest = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes(HostNetworkConverter.Int16((short)42)), 0, _IDRequest, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(HostNetworkConverter.Int16((short)TcpMessages.ID)), 0, _IDRequest, 2, 2);

            _HeartbeatAnswer = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes(HostNetworkConverter.Int16((short)42)), 0, _HeartbeatAnswer, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(HostNetworkConverter.Int16((short)TcpMessages.Heartbeat)), 0, _HeartbeatAnswer, 2, 2);
        }

        public EntityHandlerProvider(TcpServer tcpServer)
            :this()
        {
            TcpServer = tcpServer;
        }

        private void _HandleData(ConnectionState state)
        {
            switch (_IncomingMessage)
            {
                case TcpMessages.ID:
                    ID = Encoding.ASCII.GetString(_Data);
                    Console.WriteLine("Received ID from client: {0}", ID);
                    _Client = new ViewModels.NetworkClientVM(ID, state);
                    TcpServer.AddClientMapping(_Client);

                    App.Instance.MediatorService.BroadcastMessage(MediatorMessages.TcpServer_NetworkClientAdded, this, _Client);
                    goto default;
                case TcpMessages.Heartbeat:                    
                    if (!state.Write(_HeartbeatAnswer, 0, _HeartbeatAnswer.Length))
                        state.EndConnection();
                    break;
                default:
                    lock (_Client.Lock)
                        _Client.LastMessageReceived = _IncomingMessage;
                    break;
            }

            _MessageIdentified = false;
            _DataLength = 0;
            _DataReceived = 0;
        }
    }
}
