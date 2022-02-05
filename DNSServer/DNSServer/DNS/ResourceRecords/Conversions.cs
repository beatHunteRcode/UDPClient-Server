using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSServer.DNS
{
    class Conversions
    {
        public static String convertBytesArrayToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static short convertBytesArrayToShort(byte[] bytes)
        {
            return (short) BitConverter.ToUInt16(new byte[2] { (byte)bytes[1], (byte)bytes[0] }, 0);
        }

        public static Int32 convertBytesArrayToInt32(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        public static UInt16 convertBytesArrayToUInt16(byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes, 0);
        }


        public static byte convertBitsArrayToByte(BitArray bits)
        {
            BitArray helpfullBitArray;
            if (bits.Count != 8)
            {
                helpfullBitArray = new BitArray(8);
                for (int i = 0; i < bits.Length; i++) helpfullBitArray[i] = bits[i];
                for (int i = bits.Length; i < helpfullBitArray.Length; i++) helpfullBitArray[i] = false;
                byte[] bytes = new byte[1];
                helpfullBitArray.CopyTo(bytes, 0);
                return bytes[0];
            }
            else
            {
                byte[] bytes = new byte[1];
                bits.CopyTo(bytes, 0);
                return bytes[0];
            }
        }

        public static byte convertBooleanToByte(bool value)
        {
            return value ? (byte)1 : (byte)0;
        }

        public static byte[] convertShortToByteArray(short value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            byte tmp = 0;
            tmp = arr[0];
            arr[0] = arr[1];
            arr[1] = tmp;

            return arr;
        }

        public static string convertByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] convertIntToByteArray(int value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            byte tmp0 = arr[0];
            byte tmp1 = arr[1];
            byte tmp2 = arr[2];
            byte tmp3 = arr[3];

            return new byte[] { tmp3, tmp2, tmp1, tmp0 };
        }
    }
}
