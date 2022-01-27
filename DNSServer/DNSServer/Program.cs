using DNSServer.DNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer
{
    class Program
    {
        const string ip = "127.0.0.1";
        const int port = 53;

        static void Main(string[] args)
        {
            //Console.WriteLine("" +
            //    "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
            //    "|                      ID                       |\n" +
            //    "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
            //    "|QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |\n" +
            //    "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
            //    "|                    QDCOUNT                    |\n" +
            //    "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
            //    "|                    ANCOUNT                    |\n" +
            //    "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
            //    "|                    NSCOUNT                    |\n" +
            //    "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
            //    "|                    ARCOUNT                    |\n" +
            //    "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+");
            Server server = new Server(ip, port);
            server.start();
        }
    }
}
