using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer.DNS
{
    class DNSMessage
    {
        public DNSHeader _DNSHeader;
        public DNSQuery _DNSQuery;
        public DNSType _ResourceRecordType; 

        private static byte _DNSHeaderLength = 12;


        public byte[] _DNSHeaderBytesArray = new byte[_DNSHeaderLength];
        public byte[] _DNSQueryBytesArray;
        
        public DNSMessage(DNSHeader dnsheader, DNSQuery dnsquery)
        {
            _DNSHeader = dnsheader;
            _DNSQuery = dnsquery;
        }

        public DNSMessage(byte[] bytes)
        {
            Array.Copy(bytes, 0, _DNSHeaderBytesArray, 0, _DNSHeaderLength);
            _DNSQueryBytesArray = new byte[bytes.Length - 12];
            Array.Copy(bytes, _DNSHeaderLength, _DNSQueryBytesArray, 0, _DNSQueryBytesArray.Length);
            _DNSHeader = new DNSHeader(_DNSHeaderBytesArray);
            _DNSQuery = new DNSQuery(_DNSQueryBytesArray);
            _ResourceRecordType = _DNSQuery._QTYPE;
        }
    }
}
