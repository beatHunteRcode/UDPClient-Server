using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer.DNS
{
    class DNSResourceRecord
    {
        public List<byte[]> _NameArr { get; set; }
        public DNSType _Type { get; set; }
        public short _Class { get; set; }
        public Int32 _TTL { get; set; }
        public short _RDLength { get; set; }
        public byte[] _RData { get; set; }

        private static short _NameLength = 0;
        private static short _TypeLength = 2;
        private static short _ClassLength = 2;
        private static short _TTLLength = 4;
        private static short _RDLengthLength = 2;
        private static short _RDataLength = 4;


        private byte[] _nameHelpArray = new byte[_NameLength];
        private byte[] _typeHelpArray = new byte[_TypeLength];
        private byte[] _classHelpArray = new byte[_ClassLength];
        private byte[] _ttlHelpArray = new byte[_TTLLength];
        private byte[] _rdlengthHelpArray = new byte[_RDLengthLength];
        private byte[] _rdataHelpArray = new byte[_RDataLength];

        public DNSResourceRecord()
        {

        }

        public DNSResourceRecord(List<byte[]> namearr, DNSType type, short Class, Int32 ttl, short rdlength)
        {
            _NameArr = namearr;
            _Type = type;
            this._Class = Class;
            _TTL = ttl;
            _RDLength = rdlength;
            //_RData = rdata;
        }

        public DNSResourceRecord(byte[] bytes)
        {
            Array.Copy(bytes, 0, _nameHelpArray, 0, _nameHelpArray.Length);
            Array.Copy(bytes, _NameLength, _typeHelpArray, 0, _typeHelpArray.Length);
            Array.Copy(bytes, _NameLength + _ClassLength, _classHelpArray, 0, _classHelpArray.Length);
            Array.Copy(bytes, _NameLength + _ClassLength + _TTLLength, _ttlHelpArray, 0, _ttlHelpArray.Length);
            Array.Copy(bytes,   _NameLength + _ClassLength + 
                                _TTLLength + _RDLengthLength,     _rdlengthHelpArray, 0, _rdlengthHelpArray.Length);
            Array.Copy(bytes, _NameLength + _ClassLength +
                                _TTLLength + _RDLengthLength + _RDataLength, _rdataHelpArray, 0, _rdataHelpArray.Length);

            _NameArr = parseName(bytes);
            _Type = (DNSType)Conversions.convertBytesArrayToShort(_typeHelpArray);
            _Class = Conversions.convertBytesArrayToShort(_classHelpArray);
            _TTL = Conversions.convertBytesArrayToInt32(_ttlHelpArray);
            _RDLength = Conversions.convertBytesArrayToShort(_rdlengthHelpArray);
            //_RData = Conversions.convertBytesArrayToString(_rdataHelpArray);
        }

        public static List<byte[]> parseName(byte[] bytes)
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
            _NameLength = (short)(index + 1);
            return newNameBytes;
        }
    }
}
