using DNSServer.DNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DNSServer
{
    class Server
    {

        readonly string _ip;
        readonly int _port;
        IPEndPoint _UDPEndPoint;
        Socket _UDPSocket;

        private byte[] _buffer;
        private ArraySegment<byte> _bufferSegment;

        private String rrAstringIP = "77.79.249.29";
        private String rrAAAAstringIP = "3d31:6534:fe34:123a:654c:13bb:6543:451f";
        private String rrMXstring = "mx.arasaka.bad";
        private String rrTXTstring = "I never asked for this...";

        public Server(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void start()
        {
            try
            {
                initialize();
                startMessagesReceive();
            }
            finally
            {
                String exitPhrase = "";
                do
                {
                    exitPhrase = Console.ReadLine();
                }
                while (exitPhrase != "exit()");
            }
        }

        private void initialize()
        {
            _UDPEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _UDPSocket.Bind(_UDPEndPoint);
            Console.WriteLine("Server has successfully launched");
            Console.WriteLine("Server is running");
            _buffer = new byte[128];
            _bufferSegment = new ArraySegment<byte>(_buffer);
        }

        private void startMessagesReceive()
        {
            Thread userThread = new Thread(async () =>
            {
                SocketReceiveMessageFromResult res;
                while (true)
                {
                    res = await _UDPSocket.ReceiveMessageFromAsync(_bufferSegment, SocketFlags.None, _UDPEndPoint);
                    DNSMessage dnsMessage = new DNSMessage(_bufferSegment.Array);
                    printIncomindDNSMessage(dnsMessage);
                    dnsMessage._DNSHeader._QR = true;
                    DNSResourceRecord RR = createResourceRecord(dnsMessage);
                    DNSAnswer dnsAnswer = createDNSAnswer(RR);
                    dnsMessage._DNSHeader._ANCOUNT++;
                    await SendTo(res.RemoteEndPoint, dnsMessage._DNSHeader.getBytes().Concat(dnsMessage._DNSQuery.getBytes()).Concat(dnsAnswer.getBytes()).ToArray());
                    printOutcomindDNSMessage(dnsMessage, dnsAnswer);
                }
            });
            userThread.Start();
        }

        public async Task SendTo(EndPoint recipient, byte[] data)
        {
            var s = new ArraySegment<byte>(data);
            await _UDPSocket.SendToAsync(s, SocketFlags.None, recipient);
        }

        private void PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("bytes: [ ");
            foreach (var b in bytes)
            {
                sb.Append(b + " ");
            }
            sb.Append("]");
            Console.WriteLine(sb.ToString());
        }

        private DNSResourceRecord createResourceRecord(DNSMessage dnsMessage)
        {
            DNSResourceRecord resourceRecord = new DNSResourceRecord();
            switch (dnsMessage._DNSQuery._QTYPE)
            {
                case DNSType.A:
                    DNSResourceRecordA rrA = new DNSResourceRecordA(
                        dnsMessage._DNSQuery._QNameArr,
                        dnsMessage._DNSQuery._QTYPE,
                        1,
                        3600,
                        4);
                    rrA._RDATA = new List<byte>();
                    string[] arrIPv4string = rrAstringIP.Split('.');
                    for(int i = 0; i < arrIPv4string.Length; i++)
                    {
                        rrA._RDATA.Add(byte.Parse(arrIPv4string[i]));
                    }
                    resourceRecord = rrA;
                    rrA.fillParentRDATAWithBytes();
                    break;
                case DNSType.MX:
                    DNSResourceRecordMX rrMX = new DNSResourceRecordMX(
                        dnsMessage._DNSQuery._QNameArr,
                        dnsMessage._DNSQuery._QTYPE,
                        1,
                        3600,
                        0);
                    rrMX._Preference = 10;
                    rrMX._Exchange = rrMXstring;
                    rrMX._RDLength = (short)(2 + Encoding.UTF8.GetBytes(rrMX._Exchange).Length);
                    resourceRecord = rrMX;
                    rrMX.fillParentRDATAWithBytes();
                    break;
                case DNSType.AAAA:
                    DNSResourceRecordAAAA rrAAAA = new DNSResourceRecordAAAA(
                        dnsMessage._DNSQuery._QNameArr,
                        dnsMessage._DNSQuery._QTYPE,
                        1,
                        3600,
                        16);
                    rrAAAA._RDATA = new List<byte[]>();
                    string[] arrIPv6string = rrAAAAstringIP.Split(':');
                    short[] arrIPv6short = new short[arrIPv6string.Length];
                    for (int i = 0; i < arrIPv6short.Length; i++)
                    {
                        arrIPv6short[i] = Convert.ToInt16(arrIPv6string[i], 16);
                        rrAAAA._RDATA.Add(Conversions.convertShortToByteArray(arrIPv6short[i]));
                    }
                    resourceRecord = rrAAAA;
                    rrAAAA.fillParentRDATAWithBytes();
                    break;
                case DNSType.TXT:
                    DNSResourceRecordTXT rrTXT = new DNSResourceRecordTXT(
                        dnsMessage._DNSQuery._QNameArr,
                        dnsMessage._DNSQuery._QTYPE,
                        1,
                        3600,
                        0);
                    rrTXT._RDATA = rrTXTstring;
                    rrTXT._RDLength = (short)rrTXT._RDATA.Length;
                    rrTXT._TXTLength = (short)rrTXT._RDATA.Length;
                    resourceRecord = rrTXT;
                    rrTXT.fillParentRDATAWithBytes();
                    break;
            }

            return resourceRecord;
        }

        private void printIncomindDNSMessage(DNSMessage dnsMessage)
        {
            Console.WriteLine("------------------INCOMING----------------- " + GetCurrentTime());
            Console.WriteLine("HEADER:");
            Console.WriteLine(dnsMessage._DNSHeader.ToString());
            PrintByteArray(dnsMessage._DNSHeader.getBytes());
            Console.WriteLine("\n\n");
            Console.WriteLine("QUERY:");
            Console.WriteLine(dnsMessage._DNSQuery.ToString());
            PrintByteArray(dnsMessage._DNSQuery.getBytes());
            Console.WriteLine("\nALL: ");
            PrintByteArray(dnsMessage._DNSHeader.getBytes().Concat(dnsMessage._DNSQuery.getBytes()).ToArray());
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("\n\n");
        }

        private void printOutcomindDNSMessage(DNSMessage dnsMessage, DNSAnswer dnsAnswer)
        {
            Console.WriteLine("-----------------OUTCOMING----------------- " + GetCurrentTime());
            Console.WriteLine("HEADER\n" + dnsMessage._DNSHeader.ToString());
            PrintByteArray(dnsMessage._DNSHeader.getBytes());
            Console.WriteLine("\nQUERY\n" + dnsMessage._DNSQuery.ToString());
            PrintByteArray(dnsMessage._DNSQuery.getBytes());
            Console.WriteLine("\nANSWER\n" + dnsAnswer.ToString());
            PrintByteArray(dnsAnswer.getBytes());
            Console.WriteLine("\nALL: ");
            PrintByteArray(dnsMessage._DNSHeader.getBytes().Concat(dnsMessage._DNSQuery.getBytes()).Concat(dnsAnswer.getBytes()).ToArray());
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("\n\n");
        }

        private DNSAnswer createDNSAnswer(DNSResourceRecord RR)
        {
            DNSAnswer dnsAnswer = new DNSAnswer();
            dnsAnswer._NameArr = RR._NameArr;
            dnsAnswer._TYPE = RR._Type;
            dnsAnswer._CLASS = 1; //CLASS = 1 = IN = Internet
            dnsAnswer._TTL = 3600; // 3600 sec = 1 hour
            dnsAnswer._RDLENGTH = RR._RDLength;
            dnsAnswer._RDATA = RR._RData;
            return dnsAnswer;
        }

        private String GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm:ss tt");
        }
    }
}
