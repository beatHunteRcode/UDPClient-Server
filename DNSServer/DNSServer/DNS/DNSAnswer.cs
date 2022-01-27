using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNSServer.DNS
{
    class DNSAnswer
    {
        /*
         * DNS Answer section structure
         *                                      1  1  1  1  1  1
                  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                                               |
                /                                               /
                /                      NAME                     /
                |                                               |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                      TYPE                     |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                     CLASS                     |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                      TTL                      |
                |                                               |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                   RDLENGTH                    |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
                /                     RDATA                     /
                /                                               /
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         */

        public List<byte[]> _NameArr { get; set; }
        public DNSType _TYPE { get; set; }
        public short _CLASS { get; set; }
        public int _TTL { get; set; }
        public short _RDLENGTH { get; set; }
        public byte[] _RDATA { get; set; }

        public DNSAnswer()
        {

        }

        public DNSAnswer(List<byte[]> namearr, DNSType type, short Class, int ttl, short rdlength, byte[] rdata)
        {
            _NameArr = namearr;
            _TYPE = type;
            _CLASS = Class;
            _TTL = ttl;
            _RDLENGTH = rdlength;
            _RDATA = rdata;
        }

        public byte[] getBytes()
        {
            if (_RDATA == null) _RDATA = new byte[0];
            byte[] nameBytes = getQNameBytes(_NameArr);
            byte[] typeBytes = BitConverter.GetBytes((short)_TYPE);
            byte[] classBytes = BitConverter.GetBytes(_CLASS);
            byte[] ttlBytes = BitConverter.GetBytes(_TTL);
            byte[] rdlengthBytes = Conversions.convertShortToByteArray(_RDLENGTH);
            byte[] rdataBytes = _RDATA;

            byte[] answerBytes = nameBytes.
                                    Concat(nameBytes).
                                    Concat(typeBytes).
                                    Concat(classBytes).
                                    Concat(ttlBytes).
                                    Concat(rdlengthBytes).
                                    Concat(rdataBytes).ToArray();
            return answerBytes;
         }

        private byte[] getQNameBytes(List<byte[]> list)
        {
            byte[] answerBytes = new byte[0];
            foreach (byte[] el in list)
            {
                answerBytes.Concat(el);
            }
            return answerBytes;
        }

        public override string ToString()
        {
            StringBuilder stName = new StringBuilder();
            foreach (byte[] el in _NameArr)
            {
                stName.Append(Encoding.UTF8.GetString(el)).Append(".");
            }


            StringBuilder stData = new StringBuilder();
            switch (_TYPE)
            {
                case DNSType.A:
                    foreach (byte el in _RDATA)
                    {
                        stData.Append(el).Append(".");
                    }
                    break;
                case DNSType.MX:
                    byte[] prefBytes = new byte[] { _RDATA[0], _RDATA[1] };
                    byte[] exchBytes = new byte[_RDATA.Length - 2];
                    Array.Copy(_RDATA, 2, exchBytes, 0, _RDATA.Length - 2);
                    stData.Append("Pref:").Append(Conversions.convertBytesArrayToShort(prefBytes)).
                            Append("   ").Append("MailExch:").Append(Encoding.UTF8.GetString(exchBytes));
                    break;
                case DNSType.AAAA:
                    for (int i = 0; i < _RDATA.Length; i+=2)
                    {
                        byte[] octetBytes = new byte[] { _RDATA[i], _RDATA[i+1]};
                        stData.Append(Conversions.convertByteArrayToHexString(octetBytes)).Append(":");
                    }
                    break;
                case DNSType.TXT:
                    stData.Append(Conversions.convertBytesArrayToString(_RDATA));
                    break;
            }
            
            return
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                                               |\n" +
                "/                                               /\n" +
                "/                      " + stName.ToString() + "                     /\n" +
                "|                                               |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                      " + _TYPE + "                     |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                     " + _CLASS + "                     |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                      " + _TTL + "                      |\n" +
                "|                                               |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                   " + _RDLENGTH + "                   |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|\n" +
                "/                     " + stData.ToString() + "                     /\n" +
                "/                                               /\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+";
        }
    }

}
