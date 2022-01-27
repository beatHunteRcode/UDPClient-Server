using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TFTPClient.Packets;

namespace TFTPClient
{
    class Client
    {
        readonly string _ip;
        readonly int _port;
        IPEndPoint _UDPEndPoint;
        Socket _UDPSocket;

        private byte[] _buffer;
        private ArraySegment<byte> _bufferSegment;

        Dictionary<byte, string> errorCodes = new Dictionary<byte, string>
        {
            { 0, "Not defined, see error message (if any)." },
            { 1, "File not found." },
            { 2, "Access violation." },
            { 3, "Disk full or allocation exceeded." },
            { 4, "Illegal TFTP operation." },
            { 5, "Unknown transfer ID." },
            { 6, "File already exists." },
            { 7, "No such user." }
        };

        public Client(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void start()
        {
            try
            {
                initialize();
                startMessagesSend();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
        }

        private void initialize()
        {
            _UDPEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Console.WriteLine("Client is running");
            _buffer = new byte[512];
            _bufferSegment = new ArraySegment<byte>(_buffer);
        }

        private void startMessagesSend()
        {
            Thread thread = new Thread(async () =>
            {
                SocketReceiveMessageFromResult res;
                string message = "";
                do
                {
                    message = Console.ReadLine();
                    await Send(Encoding.UTF8.GetBytes(message));
                    res = await _UDPSocket.ReceiveMessageFromAsync(_bufferSegment, SocketFlags.None, _UDPEndPoint);
                    ReceiveBytes(_bufferSegment.Array);
                    Console.WriteLine(Encoding.UTF8.GetString(_bufferSegment.Array));
                }
                while (message != "exit()");
            });
            thread.Start();
        }

        public void ReceiveBytes(byte[] bytes)
        {
            byte[] opCodeArr = new byte[1];
            Array.Copy(bytes, 0, opCodeArr, 0, 2);
            OpCodeType opCodeType = (OpCodeType)Conversions.convertBytesArrayToShort(opCodeArr);
            switch(opCodeType)
            {
                case OpCodeType.RRQ:
                    break;
                case OpCodeType.WRQ:
                    break;
                case OpCodeType.DATA:
                    break;
                case OpCodeType.ACK:
                    break;
                case OpCodeType.ERROR:
                    ErrorPacket errorPacket = new ErrorPacket(bytes);
                    Console.WriteLine(GetCurrentTime() + " [SERVER]: ERROR - " + errorPacket._ErrorMsg + ", MODE - " + errorPacket._ErrorCode);
                    break;
            }
        }

        public async Task Send(byte[] data)
        {
            var s = new ArraySegment<byte>(data);
            await _UDPSocket.SendToAsync(s, SocketFlags.None, _UDPEndPoint);
        }

        private String GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm:ss tt");
        }

    }
}
