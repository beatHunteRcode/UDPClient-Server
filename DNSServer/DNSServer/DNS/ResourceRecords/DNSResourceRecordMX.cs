using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer.DNS
{
    class DNSResourceRecordMX : DNSResourceRecord
    {

        public short _Preference;
        public String _Exchange;
        public DNSResourceRecordMX(byte[] bytes) : base(bytes)
        {

        }

        public DNSResourceRecordMX(List<byte[]> name, DNSType type, short Class, Int32 ttl, short rdlength) : base(name, type, Class, ttl, rdlength)
        {

        }

        public void fillParentRDATAWithBytes()
        {
            _RData = Conversions.convertShortToByteArray(_Preference).Concat(convertDomainNameIntoByteArr(_Exchange)).ToArray();
        }

        private byte[] convertDomainNameIntoByteArr(String domainName)
        {
            byte[] answerBytes = new byte[0];
            string[] parts = domainName.Split('.');
            foreach (string part in parts)
            {
                answerBytes = answerBytes.
                    Concat(new byte[] { (byte)part.Length }).
                    Concat(Encoding.UTF8.GetBytes(part)).ToArray();
            }
            answerBytes = answerBytes.Concat(new byte[] { 0 }).ToArray();
            return answerBytes;
        }
    }
}
