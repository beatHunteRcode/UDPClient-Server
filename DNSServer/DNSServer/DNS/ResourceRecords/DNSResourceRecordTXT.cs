using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer.DNS
{
    class DNSResourceRecordTXT : DNSResourceRecord
    {

        public String _RDATA;

        public DNSResourceRecordTXT(byte[] bytes) : base(bytes)
        {
            
        }

        public DNSResourceRecordTXT(List<byte[]> name, DNSType type, short Class, Int32 ttl, short rdlength) : base(name, type, Class, ttl, rdlength)
        {
            
        }

        public void fillParentRDATAWithBytes()
        {
            _RData = Encoding.UTF8.GetBytes(_RDATA);
        }
    }
}
