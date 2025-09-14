using SendRecieveUDP.Common.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendRecieveUDP.Model;
namespace SendRecieveUDP.Service
{
    public class BitManipulator : IBitManipulator
    {
        public void WriteBits(byte[] buffer, int bitOffset, int bitCount, ulong value)
        {
            for (int indexInByte = 0; indexInByte < bitCount; indexInByte++)
            {
                int bitVal = (int)((value >> indexInByte) & 1UL);
                int byteIdx = (bitOffset + indexInByte) / ConstantSend.BITS_IN_BYTE;
                int bitIdx = (bitOffset + indexInByte) % ConstantSend.BITS_IN_BYTE;

                if (bitVal == 1)
                    buffer[byteIdx] |= (byte)(1 << bitIdx);
                else
                    buffer[byteIdx] &= (byte)~(1 << bitIdx);
            }
        }


        public ulong ReadBits(byte[] buffer, int bitOffset, int bitCount)
        {


            ulong value = 0UL;
            for (int indexInByte = 0; indexInByte < bitCount; indexInByte++)
            {
                int byteIdx = (bitOffset + indexInByte) / ConstantSend.BITS_IN_BYTE;
                int bitIdx = (bitOffset + indexInByte) % ConstantSend.BITS_IN_BYTE;
                int bit = (buffer[byteIdx] >> bitIdx) & ConstantSend.BYTE_SIZE;
                value |= ((ulong)bit << indexInByte);
            }
            return value;
        }
    }
}
