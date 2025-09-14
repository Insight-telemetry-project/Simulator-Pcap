using SendRecieveUDP.Common.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;

namespace SendRecieveUDP.Service.BitManipulation
{
    public class BitEncoder : IBitEncoder
    {
        public void WriteBits(byte[] buffer, int bitOffset, int bitCount, ulong value)
        {
            for (int indexInByte = ConstantBits.ZERO; indexInByte < bitCount; indexInByte++)
            {
                int byteIdx = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIdx = ConstantBits.ROUND_TO_BYTE - ((bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE);

                int bitVal = (int)((value >> (bitCount - ConstantBits.BYTE_SIZE - indexInByte)) & ConstantBits.BYTE_SIZE);

                if (bitVal == ConstantBits.BYTE_SIZE)
                    buffer[byteIdx] |= (byte)(ConstantBits.BYTE_SIZE << bitIdx);
                else
                    buffer[byteIdx] &= (byte)~(ConstantBits.BYTE_SIZE << bitIdx);
            }
        }

        public ulong ReadBits(byte[] buffer, int bitOffset, int bitCount)
        {
            ulong value = ConstantBits.ZERO;
            for (int indexInByte = ConstantBits.ZERO; indexInByte < bitCount; indexInByte++)
            {
                int byteIdx = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIdx = ConstantBits.ROUND_TO_BYTE - ((bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE);

                int bit = (buffer[byteIdx] >> bitIdx) & ConstantBits.BYTE_SIZE;
                value = (value << ConstantBits.BYTE_SIZE) | (ulong)bit; 
            }
            return value;
        }
    }
}
