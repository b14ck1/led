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
            //Determine the length of the byte array
            int countBus = 0;
            int countGroups = 0;

            ledEntity.LedBuses.Values.ToList().ForEach(bus =>
            {
                countBus++;
                bus.LedGroups.ForEach(group =>
                {
                    countGroups++;                    
                });
            });
            byte[] data = new byte[2 + countBus * 3 + countGroups * 3];

            //Write out the total number of busses
            int writtenBytes = 0;
            Buffer.BlockCopy(BitConverter.GetBytes((UInt16)ledEntity.LedBuses.Count), 0, data, writtenBytes, 2);
            writtenBytes += 2;

            foreach(var bus in ledEntity.LedBuses)
            {
                //Write out bus id
                data[writtenBytes] = bus.Key;
                writtenBytes++;

                //Write out number of groups in this bus
                Buffer.BlockCopy(BitConverter.GetBytes((UInt16)bus.Value.LedGroups.Count), 0, data, writtenBytes, 2);
                writtenBytes += 2;

                foreach(var group in bus.Value.LedGroups)
                {
                    //Write out group id
                    data[writtenBytes] = group.PositionInBus;
                    writtenBytes++;

                    //Write out number of leds in this group
                    Buffer.BlockCopy(BitConverter.GetBytes((UInt16)group.Leds.Count), 0, data, writtenBytes, 2);
                    writtenBytes += 2;
                }
            }

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
            //Determine the length of the byte array
            int countSeconds = ledEntity.Seconds.Length;
            int bytesOneImage = ledEntity.AllLedIDs.Count * 8;
            int countFrames = 0;
            int countLedChanges = 0;
            foreach(var second in ledEntity.Seconds)
            {
                foreach (var frame in second.Frames)
                {
                    if (frame.LedChanges.Count > 0)
                    {
                        countFrames++;
                        countLedChanges += frame.LedChanges.Count;
                    }
                }
            }
            byte[] data = new byte[2 + 2 + countSeconds * bytesOneImage + 2 + countFrames * 4 + countLedChanges * 8];

            //At first we send the number of seconds
            int writtenBytes = 0;
            Buffer.BlockCopy(BitConverter.GetBytes((UInt16)ledEntity.Seconds.Length), 0, data, writtenBytes, 2);
            writtenBytes += 2;

            //After how many frames per second
            data[writtenBytes] = Defines.FramesPerSecond;
            writtenBytes++;

            //How much bytes does one image take
            Buffer.BlockCopy(BitConverter.GetBytes((UInt16)bytesOneImage), 0, data, writtenBytes, 2);
            writtenBytes += 2;

            //Write out all images
            foreach (var second in ledEntity.Seconds)
            {                
                foreach (var led in second.LedEntityStatus)
                {
                    //First the led id
                    data[writtenBytes] = led.LedID.BusID;
                    writtenBytes++;
                    data[writtenBytes] = led.LedID.PositionInBus;
                    writtenBytes++;
                    Buffer.BlockCopy(BitConverter.GetBytes(led.LedID.Led), 0, data, writtenBytes, 2);
                    writtenBytes += 2;

                    //After the color (A,R,G,B)
                    data[writtenBytes] = led.Color.A;
                    writtenBytes++;
                    data[writtenBytes] = led.Color.R;
                    writtenBytes++;
                    data[writtenBytes] = led.Color.G;
                    writtenBytes++;
                    data[writtenBytes] = led.Color.B;
                    writtenBytes++;
                }                
            }

            //Number of frames
            Buffer.BlockCopy(BitConverter.GetBytes((UInt16)countFrames), 0, data, writtenBytes, 2);
            writtenBytes += 2;

            //Write out all frames
            for (int i = 0; i < ledEntity.Seconds.Length; i++)
            {
                for (int j = 0; j < ledEntity.Seconds[i].Frames.Length; j++)
                {
                    if (ledEntity.Seconds[i].Frames[j].LedChanges.Count > 0)
                    {
                        //Which frame
                        Buffer.BlockCopy(BitConverter.GetBytes((UInt16)(i * Defines.FramesPerSecond + j)), 0, data, writtenBytes, 2);
                        writtenBytes += 2;

                        //How many led changes
                        Buffer.BlockCopy(BitConverter.GetBytes((UInt16)ledEntity.Seconds[i].Frames[j].LedChanges.Count), 0, data, writtenBytes, 2);
                        writtenBytes += 2;

                        foreach (var led in ledEntity.Seconds[i].Frames[j].LedChanges)
                        {
                            //First the led id
                            data[writtenBytes] = led.LedID.BusID;
                            writtenBytes++;
                            data[writtenBytes] = led.LedID.PositionInBus;
                            writtenBytes++;
                            Buffer.BlockCopy(BitConverter.GetBytes(led.LedID.Led), 0, data, writtenBytes, 2);
                            writtenBytes += 2;

                            //After the color (A,R,G,B)
                            data[writtenBytes] = led.Color.A;
                            writtenBytes++;
                            data[writtenBytes] = led.Color.R;
                            writtenBytes++;
                            data[writtenBytes] = led.Color.G;
                            writtenBytes++;
                            data[writtenBytes] = led.Color.B;
                            writtenBytes++;
                        }
                    }
                }
            }

            _TcpServer.SendData(TcpMessages.RenderedEffects, data, id);
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
                _UdpSocket.SendAnswer(ip, port);
        }
    }
}
