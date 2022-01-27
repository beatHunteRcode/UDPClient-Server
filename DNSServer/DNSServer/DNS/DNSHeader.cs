using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer.DNS
{
    class DNSHeader
    {
        /*
         * DNS Header structure
         *                                      1  1  1  1  1  1
                  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                      ID                       |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                    QDCOUNT                    |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                    ANCOUNT                    |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                    NSCOUNT                    |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                |                    ARCOUNT                    |
                +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
         */
        public short _ID { get; set; }
        public bool _QR { get; set; }
        public byte _Opcode { get; set; }
        public bool _AA { get; set; }
        public bool _TC { get; set; }
        public bool _RD { get; set; }
        public bool _RA { get; set; }
        public byte _Z { get; set; }
        public byte _RCODE { get; set; }
        public short _QDCOUNT { get; set; }
        public short _ANCOUNT { get; set; }
        public short _NSCOUNT { get; set; }
        public short _ARCOUNT { get; set; }

        private static byte _IDLength = 2;          // 2 bytes
        private static byte _FlagsLength = 2;       // 2 bytes
        private static byte _OpcodeLength = 4;      // 4 bits (!)
        private static byte _ZLength = 3;           // 3 bits (!)
        private static byte _RCODELength = 4;       // 4 bits (!)
        private static short _QDCOUNTLength = 2;    // 2 bytes
        private static short _ANCOUNTLength = 2;    // 2 bytes
        private static short _NSCOUNTLength = 2;    // 2 bytes
        private static short _ARCOUNTLength = 2;    // 2 bytes

        private byte[] _IDHelpArray = new byte[_IDLength];
        private byte[] _FlagsHelpArray = new byte[_FlagsLength];
        private BitArray _OpcodeHelpArray = new BitArray(_OpcodeLength);
        private BitArray _ZHelpArray = new BitArray(_ZLength);
        private BitArray _RCODEHelpArray = new BitArray(_RCODELength);
        private byte[] _QDCOUNTHelpArray = new byte[_QDCOUNTLength];
        private byte[] _ANCOUNTHelpArray = new byte[_ANCOUNTLength];
        private byte[] _NSCOUNTHelpArray = new byte[_NSCOUNTLength];
        private byte[] _ARCOUNTHelpArray = new byte[_ARCOUNTLength];

        private int flagsSpace = 0;

        public DNSHeader(   short id, bool qr, byte opcode, bool aa, bool tc, bool rd, bool ra, 
                            byte z, byte rcode, short qdcount, short ancount, short nscount, short arcount)
        {
            _ID = id;
            _QR = qr;
            _Opcode = opcode;
            _AA = aa;
            _TC = tc;
            _RD = rd;
            _RA = ra;
            _Z = z;
            _RCODE = rcode;
            _QDCOUNT = qdcount;
            _ANCOUNT = ancount;
            _NSCOUNT = nscount;
            _ARCOUNT = arcount;
        }

        public DNSHeader(byte[] bytes)
        {
            Array.Copy(bytes, 0, _IDHelpArray, 0, _IDHelpArray.Length);
            Array.Copy(bytes, _IDLength, _FlagsHelpArray, 0, _FlagsHelpArray.Length);
            Array.Copy(bytes,   _IDLength + 2, _QDCOUNTHelpArray, 0, _QDCOUNTHelpArray.Length);
            Array.Copy(bytes,   _IDLength + 2 + _QDCOUNTLength, _ANCOUNTHelpArray, 0, _ANCOUNTHelpArray.Length);
            Array.Copy(bytes,   _IDLength + 2 + _QDCOUNTLength + _ANCOUNTLength, _NSCOUNTHelpArray, 0, _NSCOUNTHelpArray.Length);
            Array.Copy(bytes,   _IDLength + 2 + _QDCOUNTLength + _ANCOUNTLength + _NSCOUNTLength, _ARCOUNTHelpArray, 0, _ARCOUNTHelpArray.Length);

            BitArray flagsBits = new BitArray(_FlagsHelpArray);

            _ID = Conversions.convertBytesArrayToShort(_IDHelpArray);
            _QR = flagsBits[0];
            _OpcodeHelpArray.Set(0, flagsBits[1]);
            _OpcodeHelpArray.Set(1, flagsBits[2]);
            _OpcodeHelpArray.Set(2, flagsBits[3]);
            _OpcodeHelpArray.Set(3, flagsBits[4]);
            _Opcode = Conversions.convertBitsArrayToByte(_OpcodeHelpArray);
            _AA = flagsBits[5];
            _TC = flagsBits[6];
            _RD = flagsBits[7];
            _RA = flagsBits[8];
            _ZHelpArray.Set(0, flagsBits[9]);
            _ZHelpArray.Set(1, flagsBits[10]);
            _ZHelpArray.Set(2, flagsBits[11]);
            _Z = Conversions.convertBitsArrayToByte(_ZHelpArray);
            _RCODEHelpArray.Set(0, flagsBits[12]);
            _RCODEHelpArray.Set(1, flagsBits[13]);
            _RCODEHelpArray.Set(2, flagsBits[14]);
            _RCODEHelpArray.Set(3, flagsBits[15]);
            _RCODE = Conversions.convertBitsArrayToByte(_RCODEHelpArray);
            _QDCOUNT = Conversions.convertBytesArrayToShort(_QDCOUNTHelpArray);
            _ANCOUNT = Conversions.convertBytesArrayToShort(_ANCOUNTHelpArray);
            _NSCOUNT = Conversions.convertBytesArrayToShort(_NSCOUNTHelpArray);
            _ARCOUNT = Conversions.convertBytesArrayToShort(_ARCOUNTHelpArray);
        }

        override public String ToString()
        {
            String str =
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                      " + _ID + "                       |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|" + Conversions.convertBooleanToByte(_QR) + "|    " + _Opcode + "  |" + Conversions.convertBooleanToByte(_AA) + "|" + Conversions.convertBooleanToByte(_TC) + "|" + Conversions.convertBooleanToByte(_RD) + "|" + Conversions.convertBooleanToByte(_RA) + "|   " + _Z + "    |   " + _RCODE + "   |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                    " + _QDCOUNT + "                    |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                    " + _ANCOUNT + "                    |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                    " + _NSCOUNT + "                    |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+\n" +
                "|                    " + _ARCOUNT + "                    |\n" +
                "+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+";
            return str;
        }

        private void putFlagBit(bool flag)
        {
            flagsSpace = flagsSpace << 1;
            if (flag) flagsSpace += 1;
        }

        private void putFlagByte(byte flagByte, byte shift)
        {
            flagsSpace = flagsSpace << shift;
            flagsSpace += flagByte & 65535;
        }

        private void fixHeaderForClient(ref byte[] arr)
        {
            byte tmp = 0;
            
        }

        public byte[] getBytes()
        {
            byte[] idBytes = Conversions.convertShortToByteArray(_ID);
            putFlagBit(_QR);
            putFlagByte(_Opcode, 4);
            putFlagBit(_AA);
            putFlagBit(_TC);
            putFlagBit(_RD);
            putFlagBit(_RA);
            putFlagByte(_Z, 3);
            putFlagByte(_RCODE, 4);
            byte[] flagsBytes = Conversions.convertShortToByteArray((short)flagsSpace); 
            byte[] qdcountBytes = Conversions.convertShortToByteArray(_QDCOUNT);
            byte[] ancountBytes = Conversions.convertShortToByteArray(_ANCOUNT);
            byte[] nscountBytes = Conversions.convertShortToByteArray(_NSCOUNT);
            byte[] arcountBytes = Conversions.convertShortToByteArray(_ARCOUNT);

            byte[] answerBytes = idBytes.
                                    Concat(flagsBytes).
                                    Concat(qdcountBytes).
                                    Concat(ancountBytes).
                                    Concat(nscountBytes).
                                    Concat(arcountBytes).ToArray();
            return answerBytes;
        }
    }
}
