using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Services.lib.TCP
{
    public class EntityMessage : Comparer<EntityMessage>
    {
        static byte[] _Secret { get; }

        public TcpMessages TcpMessage { get; private set; }

        public byte[] Data { get; private set; }

        static EntityMessage()
        {
            _Secret = BitConverter.GetBytes(HostNetworkConverter.Int16((short)42));
        }

        public EntityMessage(TcpMessages tcpMessage, byte[] data)
        {
            TcpMessage = tcpMessage;
            byte[] _message = BitConverter.GetBytes(HostNetworkConverter.Int16((Int16)tcpMessage));

            byte[] _length;
            if (data == null)
            {
                _length = BitConverter.GetBytes((Int32)0);
                Data = new byte[4 + _length.Length];

                Buffer.BlockCopy(_Secret, 0, Data, 0, 2);
                Buffer.BlockCopy(_message, 0, Data, 2, 2);
                Buffer.BlockCopy(_length, 0, Data, 4, _length.Length);
            }
            else
            {
                _length = BitConverter.GetBytes(HostNetworkConverter.Int32((Int32)data.Length));
                Data = new byte[4 + _length.Length + data.Length];

                Buffer.BlockCopy(_Secret, 0, Data, 0, 2);
                Buffer.BlockCopy(_message, 0, Data, 2, 2);
                Buffer.BlockCopy(_length, 0, Data, 4, _length.Length);
                Buffer.BlockCopy(data, 0, Data, 4 + _length.Length, data.Length);
            }            
        }

        public override int Compare(EntityMessage x, EntityMessage y)
        {
            if (x.TcpMessage > y.TcpMessage)
                return 1;
            else if (x.TcpMessage < y.TcpMessage)
                return -1;
            else
                return 0;
        }
    }
}
