using SendRecieveUDP.Common.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
namespace SendRecieveUDP.Service.BitManipulation
{
    public class BitManipulator : IBitManipulator
    {
        public void WriteBits(byte[] buffer, int bitOffset, int bitCount, ulong value)
        {
            for (int indexInByte = ConstantBits.ZERO; indexInByte < bitCount; indexInByte++)
            {
                int bitVal = (int)(value >> indexInByte & ConstantTypes.ONE_LONG_SIZE);
                int byteIdx = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIdx = (bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE;

                if (bitVal == ConstantBits.BYTE_SIZE)
                    buffer[byteIdx] |= (byte)(ConstantBits.BYTE_SIZE << bitIdx);
                else
                    buffer[byteIdx] &= (byte)~(ConstantBits.BYTE_SIZE << bitIdx);
            }
        }


        public ulong ReadBits(byte[] buffer, int bitOffset, int bitCount)
        {

            ulong value = ConstantTypes.ZERO_LONG_SIZE;
            for (int indexInByte = ConstantBits.ZERO; indexInByte < bitCount; indexInByte++)
            {
                int byteIdx = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIdx = (bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE;
                int bit = buffer[byteIdx] >> bitIdx & ConstantBits.BYTE_SIZE;
                value |= (ulong)bit << indexInByte;
            }
            return value;
        }
    }
}
