using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTPClient.Packets
{
    class ErrorPacket
    {
        /*
         *                  Error packet
         * 
         *     2 bytes     2 bytes      string    1 byte
               -----------------------------------------
              | Opcode |  ErrorCode |   ErrMsg   |   0  |
               -----------------------------------------
         * 
         */
        public OpCodeType _Opcode;
        public short _ErrorCode;
        public string _ErrorMsg;
        public const byte _Zero = 0;

        private static byte opCodeLength = 2;
        private static byte errorCodeLength = 2;
        

        public ErrorPacket()
        {
        }

        public ErrorPacket(OpCodeType opcode, short errorCode, string errorMsg)
        {
            _Opcode = opcode;
            _ErrorCode = errorCode;
            _ErrorMsg = errorMsg;
        }

        public ErrorPacket(byte[] bytes)
        {
            byte[] opcodeBytes = new byte[opCodeLength];
            byte[] errCodeBytes = new byte[errorCodeLength];
            byte[] errorMsgBytes = new byte[bytes.Length - opCodeLength - errorCodeLength];

            Array.Copy(bytes, 0, opcodeBytes, 0, opcodeBytes.Length);
            Array.Copy(bytes, opCodeLength, errCodeBytes, 0, errCodeBytes.Length);
            Array.Copy(bytes, opCodeLength + errorCodeLength, errorMsgBytes, 0, errorMsgBytes.Length);

            _Opcode = (OpCodeType)Conversions.convertBytesArrayToShort(opcodeBytes);
            _ErrorCode = Conversions.convertBytesArrayToShort(errCodeBytes);
            _ErrorMsg = Encoding.UTF8.GetString(Conversions.parseBytes(errorMsgBytes, _Zero));
        }

        public byte[] getBytes()
        {
            byte[] opcodeBytes = Conversions.convertShortToByteArray((short)_Opcode);
            byte[] errorcodeBytes = Conversions.convertShortToByteArray(_ErrorCode);
            byte[] errormsgBytes = Encoding.UTF8.GetBytes(_ErrorMsg);

            return opcodeBytes.Concat(errorcodeBytes).Concat(errormsgBytes).Concat(new byte[] { 0 }).ToArray();
        }
    }
}
