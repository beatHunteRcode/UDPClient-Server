using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer.DNS
{
    class DNSQuery
    {
        /*
         * DNS Query section structure
         *                                  1  1  1  1  1  1
              0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                                               |
            /                     QNAME                     /
            /                                               /
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                     QTYPE                     |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                     QCLASS                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         */
        public List<byte[]> _QNameArr { get; set; }
        public DNSType _QTYPE { get; set; }
        public short _QCLASS { get; set; }

        private static byte _QTYPELength = 2;
        private static byte _QCLASSLength = 2;

        private short _QNAMELength = 0;
        private byte[] _QTYPEHelpArray = new byte[_QTYPELength];
        private byte[] _QCLASSHelpArray = new byte[_QCLASSLength];

        public DNSQuery(List<byte[]> qnamearr, DNSType qtype, short qclass)
        {
            _QNameArr = qnamearr;
            _QTYPE = qtype;
            _QCLASS = qclass;
        }

        public DNSQuery(byte[] bytes)
        {
            _QNameArr = parseName(bytes);
            Array.Copy(bytes, _QNAMELength, _QTYPEHelpArray, 0, _QTYPELength);
            Array.Copy(bytes, _QNAMELength + _QTYPELength, _QCLASSHelpArray, 0, _QCLASSLength);

            
            _QTYPE = (DNSType)Conversions.convertBytesArrayToShort(_QTYPEHelpArray);
            _QCLASS = Conversions.convertBytesArrayToShort(_QCLASSHelpArray);
        }

        public List<byte[]> parseName(byte[] bytes)
        {
            int index = 0;
            List<byte[]> newNameBytes = new List<byte[]>();
            do
            {
                int limit = bytes[index];
                newNameBytes.Add(new byte[limit]);
                for (int i = 0; i < limit; i++)
                {
                    index++;
                    newNameBytes.Last()[i] = bytes[index];
                }
                index++;
            } while (bytes[index] != 0);
            _QNAMELength = (short)(index + 1);
            return newNameBytes;
        }

        private byte[] getQNameBytes(List<byte[]> list)
        {
            byte[] answerBytes = new byte[0];
            foreach (byte[] el in list) {
                answerBytes = answerBytes.Concat(new byte[] { (byte)el.Length }).Concat(el).ToArray();
            }
            answerBytes = answerBytes.Concat(new byte[] { 0 }).ToArray();
            return answerBytes;
        }

        public byte[] getBytes()
        {
            byte[] nameBytes = getQNameBytes(_QNameArr);
            byte[] typeBytes = Conversions.convertShortToByteArray((short)_QTYPE);
            byte[] classBytes = Conversions.convertShortToByteArray(_QCLASS);

            byte[] answerBytes = nameBytes.Concat(typeBytes).Concat(classBytes).ToArray();
            return answerBytes;
        }

        override public String ToString()
        {
            StringBuilder st = new StringBuilder();
            foreach (byte[] el in _QNameArr)
            {
                st.Append(Encoding.UTF8.GetString(el)).Append(".");
            }
            
            return
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                                               |\n" +
                "/                     " + st.ToString() + "                     /\n" +
                "/                                               /\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                     " + _QTYPE + "                     |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                     " + _QCLASS + "                    |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+";
        }
    }
}
