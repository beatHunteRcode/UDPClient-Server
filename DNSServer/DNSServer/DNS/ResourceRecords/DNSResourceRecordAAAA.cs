using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer.DNS
{
    class DNSResourceRecordAAAA : DNSResourceRecord
    {

        public List<byte[]> _RDATA = new List<byte[]>(16);

        public DNSResourceRecordAAAA(byte[] bytes) : base(bytes)
        {
            
        }

        public DNSResourceRecordAAAA(List<byte[]> name, DNSType type, short Class, Int32 ttl, short rdlength) : base(name, type, Class, ttl, rdlength)
        {
           
        }

        public void fillParentRDATAWithBytes()
        {
            byte[] arr = new byte[0];
            foreach (byte[] el in _RDATA)
            {
                arr = arr.Concat(el).ToArray();
            }
            _RData = arr;
        }
    }
}
