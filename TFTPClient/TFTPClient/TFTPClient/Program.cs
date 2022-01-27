using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTPClient
{
    class Program
    {
        const string ip = "127.0.0.1";
        const int port = 69; // Nice.

        static void Main(string[] args)
        {
            Client server = new Client(ip, port);
            server.start();
        }
    }
}
