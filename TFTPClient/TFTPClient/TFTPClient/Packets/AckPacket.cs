using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTPClient.Packets
{
    class AckPacket
    {
        /*
         *              Acknowledgement packet
         *      
         *               2 bytes     2 bytes
                         ---------------------
                        | Opcode |   Block #  |
                         ---------------------
         */

        public OpCodeType _Opcode;
        public short _BlockNumber;

        private static byte opCodeLength = 2;
        private static byte blockNumberLength = 2;

        public AckPacket()
        {
        }

        public AckPacket(OpCodeType opcode, short blockNumber)
        {
            _Opcode = opcode;
            _BlockNumber = blockNumber;
        }

        public AckPacket(byte[] bytes)
        {
            byte[] opcodeBytes = new byte[opCodeLength];
            byte[] blockNumberBytes = new byte[blockNumberLength];

            Array.Copy(bytes, 0, opcodeBytes, 0, opcodeBytes.Length);
            Array.Copy(bytes, opCodeLength, blockNumberBytes, 0, blockNumberBytes.Length);

            _Opcode = (OpCodeType)Conversions.convertBytesArrayToShort(opcodeBytes);
            _BlockNumber = Conversions.convertBytesArrayToShort(blockNumberBytes);
        }

        public byte[] getBytes()
        {
            byte[] opcodeBytes = Conversions.convertShortToByteArray((short)_Opcode);
            byte[] blocknumberBytes = Conversions.convertShortToByteArray(_BlockNumber);

            return opcodeBytes.Concat(blocknumberBytes).ToArray();
        }
    }
}
