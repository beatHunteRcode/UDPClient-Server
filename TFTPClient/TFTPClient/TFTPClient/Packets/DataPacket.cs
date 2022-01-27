using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTPClient.Packets
{
    class DataPacket
    {
        /*
         *                  Data packet
         *      
         *         2 bytes     2 bytes      n bytes
                   ----------------------------------
                  | Opcode |   Block #  |   Data     |
                   ----------------------------------
         * 
         */
        public OpCodeType _Opcode;
        public short _BlockNumber;
        public byte[] _Data;

        private static byte opCodeLength = 2;
        private static byte blockNumberLength = 2;

        public DataPacket()
        {
        }

        public DataPacket(OpCodeType opcode, short blockNumber, byte[] data)
        {
            _Opcode = opcode;
            _BlockNumber = blockNumber;
            _Data = data;
        }

        public DataPacket(byte[] bytes)
        {
            byte[] opcodeBytes = new byte[opCodeLength];
            byte[] blockNumberBytes = new byte[blockNumberLength];
            byte[] dataBytes = new byte[bytes.Length - opCodeLength - blockNumberLength];

            Array.Copy(bytes, 0, opcodeBytes, 0, opcodeBytes.Length);
            Array.Copy(bytes, opCodeLength, blockNumberBytes, 0, blockNumberBytes.Length);
            Array.Copy(bytes, opCodeLength + blockNumberLength, dataBytes, 0, dataBytes.Length);

            _Opcode = (OpCodeType)Conversions.convertBytesArrayToShort(opcodeBytes);
            _BlockNumber = Conversions.convertBytesArrayToShort(blockNumberBytes);
            _Data = dataBytes;
        }

        public byte[] getBytes()
        {
            byte[] opcodeBytes = Conversions.convertShortToByteArray((short)_Opcode);
            byte[] blocknumberBytes = Conversions.convertShortToByteArray(_BlockNumber);

            return opcodeBytes.Concat(blocknumberBytes).Concat(_Data).ToArray();
        }
    }
}
