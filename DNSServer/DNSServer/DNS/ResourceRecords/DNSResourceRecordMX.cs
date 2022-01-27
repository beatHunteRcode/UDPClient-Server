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
            _RData = Conversions.convertShortToByteArray(_Preference).Concat(Encoding.UTF8.GetBytes(_Exchange)).ToArray();
        }
    }
}
