using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer.DNS
{
    class DNSResourceRecordTXT : DNSResourceRecord
    {

        public short _TXTLength;
        public String _RDATA;

        public DNSResourceRecordTXT(byte[] bytes) : base(bytes)
        {
            
        }

        public DNSResourceRecordTXT(List<byte[]> name, DNSType type, short Class, Int32 ttl, short rdlength) : base(name, type, Class, ttl, rdlength)
        {
            _TXTLength = rdlength;
        }

        public void fillParentRDATAWithBytes()
        {

            _RData = Conversions.convertShortToByteArray(_TXTLength).Concat(Encoding.UTF8.GetBytes(_RDATA)).ToArray();
        }
    }
}
