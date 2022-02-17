using System;
using System.Collections.Generic;
using System.IO;
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

        // -sendfile input/ricardo_dj.png netascii
        // -readfile shrek.png netascii

        readonly string _ip;
        readonly int _port;
        EndPoint _UDPEndPoint;
        Socket _UDPSocket;

        private byte[] _buffer;
        private ArraySegment<byte> _bufferSegment;
        private List<byte[]> receivingFileSegments = new List<byte[]>();
        private string receivedFileName = "received_file";

        /*
         *                  2 bytes         2 bytes        512 bytes
         * buffer size = Opcode.Length + Block#.Length + Data.Length
         */
        private const short _bufferSize = 2 + 2 + 512; 

        private bool receivingFile = false;
        private volatile Dictionary<int, bool> dataPacketsAcksDictionary = new Dictionary<int, bool>();

        Dictionary<short, string> errorCodes = new Dictionary<short, string>
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
            _buffer = new byte[_bufferSize];
            _bufferSegment = new ArraySegment<byte>(_buffer);
        }

        private void startMessagesSend()
        {
            Thread thread = new Thread(() =>
            {
                string message = "";
                do
                {
                    message = Console.ReadLine();
                    string option = message.Split(' ')[0];
                    _UDPEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
                    _UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    switch (option)
                    {
                        case "-readfile":
                            receivedFileName = message.Split(' ')[1];
                            SendReadRequestAsync(message);
                            ReceiveMessageAsync(); // receiving ACK packet
                            do
                            {
                                ReceiveMessageAsync(); // receivind DATA packet (with needed file)
                            } while (receivingFile);
                            break;
                        case "-sendfile":
                            do
                            {
                                SendFileInfoAsync(message);
                                ReceiveMessageAsync(); // receiving ACK packet
                                SendFileAsync(message);
                            } while (dataPacketsAcksDictionary.ContainsValue(false));
                            Console.WriteLine("Succesfully sent file: " + message.Split(' ')[1]);
                            break;
                        
                    }
                }
                while (message != "exit()");
            });
            thread.Start();

            //Thread receiveThread = new Thread(() =>
            //{
            //    do
            //    {
            //        SocketReceiveMessageFromResult res = new SocketReceiveMessageFromResult();
            //        if (!_UDPSocket.Connected) _UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //        ReceiveMessageAsync(res);
            //    }
            //    while (true);
            //});
            //receiveThread.Start();
        }

        private void SendReadRequestAsync(string message)
        {
            RQPacket rqPacket = new RQPacket(OpCodeType.RRQ, message.Split(' ')[1], message.Split(' ')[2]);
            Send(rqPacket.getBytes());
        }

        public void HandleMessage(byte[] bytes)
        {
            byte[] opCodeArr = new byte[2];
            Array.Copy(bytes, 0, opCodeArr, 0, 2);
            OpCodeType opCodeType = (OpCodeType)Conversions.convertBytesArrayToShort(opCodeArr);
            switch(opCodeType)
            {
                case OpCodeType.RRQ:
                    AckPacket ackPacketRRQ = new AckPacket(OpCodeType.ACK, 0);
                    Send(ackPacketRRQ.getBytes());
                    break;
                case OpCodeType.WRQ:
                    AckPacket ackPacketWRQ = new AckPacket(OpCodeType.ACK, 0);
                    Send(ackPacketWRQ.getBytes());
                    break;
                case OpCodeType.DATA:
                    DataPacket dataPacket = new DataPacket(bytes);
                    if (dataPacket._BlockNumber == 1) receivingFile = true;
                    if (receivingFile)
                    {
                        receivingFileSegments.Add(dataPacket._Data);
                        AckPacket ackPacketData = new AckPacket(OpCodeType.ACK, dataPacket._BlockNumber);
                        //Console.WriteLine("Log: Successfully received block " + dataPacket._BlockNumber + "[" + dataPacket._Data.Length + " bytes]");
                        Send(ackPacketData.getBytes());
                    }
                    if (dataPacket._Data.Length >= 0 && dataPacket._Data.Length < 512)
                    {
                        receivingFile = false;
                        SaveFile();
                        receivingFileSegments.Clear();
                    }
                    break;
                case OpCodeType.ACK:
                    AckPacket ackPacketReceiveData = new AckPacket(bytes);
                    if (ackPacketReceiveData._BlockNumber > 0)
                    {
                        dataPacketsAcksDictionary[ackPacketReceiveData._BlockNumber] = true;
                        //Console.WriteLine("Log: Successfully sent block " + ackPacketReceiveData._BlockNumber);
                    }
                    break;
                case OpCodeType.ERROR:
                    ErrorPacket errorPacket = new ErrorPacket(bytes);
                    Console.WriteLine(
                        GetCurrentTime() + " [SERVER]: ERROR WITH CODE " + 
                        errorPacket._ErrorCode + " \"" + errorCodes[errorPacket._ErrorCode] + "\" (" + 
                        errorPacket._ErrorMsg + ")"
                    );
                    break;
            }
        }

        public void Send(byte[] data)
        {
            //var s = new ArraySegment<byte>(data);
             _UDPSocket.SendTo(data, SocketFlags.None, _UDPEndPoint);
        }

        private void ReceiveMessageAsync()
        {
            Array.Clear(_bufferSegment.Array, 0, _bufferSegment.Array.Length);
            int receivedBytesNumber = _UDPSocket.ReceiveFrom(_bufferSegment.Array, SocketFlags.None, ref _UDPEndPoint);
            List<byte> bytesList = new List<byte>();
            for (int i = 0; i < receivedBytesNumber; i++)
            {
                bytesList.Add(_bufferSegment.Array[i]);
            }
            HandleMessage(bytesList.ToArray());
        }

        private String GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm:ss tt");
        }

        public void SendFileAsync(string message)
        {
            dataPacketsAcksDictionary.Clear();
            string path = message.Split(' ')[1];
            byte[] fileBytes;
            using (FileStream fstream = File.OpenRead($"{path}"))
            {
                fileBytes = new byte[fstream.Length];
                fstream.Read(fileBytes, 0, fileBytes.Length);
            }

            List<byte[]> fileBytesSegmentsList = new List<byte[]>();
            int i = 0;
            while(i < fileBytes.Length - 1)
            {
                List<byte> segmentList = new List<byte>();
                for (int j = 0; j < _bufferSize; j++)
                {
                    segmentList.Add(fileBytes[i]);
                    i++;
                    if (i >= fileBytes.Length) break;
                }
                fileBytesSegmentsList.Add(segmentList.ToArray());
            }

            for (i = 0; i < fileBytesSegmentsList.Count; i++)
            {
                DataPacket dataPacket = new DataPacket(OpCodeType.DATA, (short)(i+1), fileBytesSegmentsList[i]);
                Send(dataPacket.getBytes());
                dataPacketsAcksDictionary.Add(i+1, false);
                ReceiveMessageAsync();
            }
        }

        private void SendFileInfoAsync(string message)
        {
            string path = message.Split(' ')[1];
            FileInfo fileInfo = new FileInfo(path);
            RQPacket rqPacket = new RQPacket(OpCodeType.WRQ, fileInfo.Name, message.Split(' ')[2]);
            byte[] packetBytes = rqPacket.getBytes();
            Send(packetBytes);
        }

        private void SaveFile()
        {
            List<byte> receivedFileBytesList = new List<byte>();
            foreach (byte[] el in receivingFileSegments)
            {
                foreach (byte b in el)
                {
                    receivedFileBytesList.Add(b);
                }
            }
            using (FileStream fstream = new FileStream($"{receivedFileName}", FileMode.OpenOrCreate))
            {
                fstream.Write(receivedFileBytesList.ToArray(), 0, receivedFileBytesList.Count);
                Console.WriteLine("Received a file from server: " + receivedFileName);
            }
        }

        private void fillAcksDictionary(Dictionary<int, bool> dictionary, int segmentsNumber)
        {
            for(int i = 1; i < segmentsNumber; i++)
            {
                dictionary.Add(i, false);
            }
        }
    }
}
