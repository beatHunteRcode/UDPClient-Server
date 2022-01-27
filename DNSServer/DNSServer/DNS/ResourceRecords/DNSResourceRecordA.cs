using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer.DNS
{
    class DNSResourceRecordA : DNSResourceRecord
    {

        public List<byte> _RDATA = new List<byte>(4);

        public DNSResourceRecordA(byte[] bytes) : base(bytes)
        {

        }

        public DNSResourceRecordA(List<byte[]> name, DNSType type, short Class, Int32 ttl, short rdlength) : base(name, type, Class, ttl, rdlength)
        {

        }

        public void fillParentRDATAWithBytes()
        {
            byte[] arr = _RDATA.ToArray();
            _RData = arr;
        }
    }
}
