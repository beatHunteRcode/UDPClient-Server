using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTPClient.Packets
{
    class RQPacket
    {
        /*
         *                              RRQ/WRQ packet
         * 
         *              2 bytes     string    1 byte     string   1 byte
                        ------------------------------------------------
                       | Opcode |  Filename  |   0  |    Mode    |   0  |
                        ------------------------------------------------
         * 
         */
        public OpCodeType _Opcode;
        public string _Filename;
        public const byte _FirstZero = 0;
        public string _Mode;
        public const byte _SecondZero = 0;


        private int opCodeLength;

        public RQPacket()
        {
        }

        public RQPacket(OpCodeType opcode, string filename, string mode)
        {
            _Opcode = opcode;
            _Filename = filename;
            _Mode = mode;
        }

        public RQPacket(byte[] bytes)
        {
            byte[] opcodeBytes = new byte[opCodeLength];
            byte[] filenameHelpArray = new byte[bytes.Length - opCodeLength]; 

            Array.Copy(bytes, 0, opcodeBytes, 0, opcodeBytes.Length);
            Array.Copy(bytes, opCodeLength, filenameHelpArray, 0, filenameHelpArray.Length);
            byte[] filenameBytes = Conversions.parseBytes(filenameHelpArray, _FirstZero);

            byte[] modeHelpArray = new byte[bytes.Length - opCodeLength - filenameBytes.Length];
            Array.Copy(bytes, opCodeLength + filenameBytes.Length + 1, modeHelpArray, 0, modeHelpArray.Length);
            byte[] modeBytes = Conversions.parseBytes(modeHelpArray, _SecondZero);

            _Opcode = (OpCodeType)Conversions.convertBytesArrayToShort(opcodeBytes);
            _Filename = Encoding.UTF8.GetString(filenameBytes);
            _Mode = Encoding.UTF8.GetString(modeBytes);
        }

        public byte[] getBytes()
        {
            byte[] opcodeBytes = Conversions.convertShortToByteArray((short)_Opcode);
            byte[] filenameBytes = Encoding.UTF8.GetBytes(_Filename);
            byte[] modeBytes = Encoding.UTF8.GetBytes(_Mode);

            return opcodeBytes.Concat(filenameBytes).Concat(new byte[] { 0 }).Concat(modeBytes).Concat(new byte[] { 0 }).ToArray();
        }
    }
}
