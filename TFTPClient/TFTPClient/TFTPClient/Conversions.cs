using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTPClient
{
    class Conversions
    {
        public static short convertBytesArrayToShort(byte[] bytes)
        {
            return (short)BitConverter.ToUInt16(new byte[2] { (byte)bytes[1], (byte)bytes[0] }, 0);
        }

        public static byte[] parseBytes(byte[] bytes, byte limit)
        {
            List<byte> newList = new List<byte>();
            foreach (byte el in bytes)
            {
                if (el != limit) newList.Add(el);
            }
            return newList.ToArray();
        }
    }
}
