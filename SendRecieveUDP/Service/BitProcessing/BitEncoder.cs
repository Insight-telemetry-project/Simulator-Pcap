using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;

namespace SendRecieveUDP.Service.BitManipulation
{
    public class BitEncoder : IBitEncoder
    {
        public void WriteBits(byte[] buffer, int bitOffset, int bitCount, ulong value)
        {
            for (int indexInByte = 0; indexInByte < bitCount; indexInByte++)
            {
                int byteIdx = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIdx = ConstantBits.MAX_BIT_INDEX_IN_BYTE - ((bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE);

                int bitVal = (int)((value >> (bitCount - ConstantBits.SINGLE_BIT_VALUE - indexInByte)) & ConstantBits.SINGLE_BIT_VALUE);

                if (bitVal == ConstantBits.SINGLE_BIT_VALUE)
                    buffer[byteIdx] |= (byte)(ConstantBits.SINGLE_BIT_VALUE << bitIdx);
                else
                    buffer[byteIdx] &= (byte)~(ConstantBits.SINGLE_BIT_VALUE << bitIdx);
            }
        }

        public ulong ReadBits(byte[] buffer, int bitOffset, int bitCount)
        {
            ulong value = ConstantBits.NO_OFFSET;
            for (int indexInByte = 0; indexInByte < bitCount; indexInByte++)
            {
                int byteIdx = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIdx = ConstantBits.MAX_BIT_INDEX_IN_BYTE - ((bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE);

                int bit = (buffer[byteIdx] >> bitIdx) & ConstantBits.SINGLE_BIT_VALUE;
                value = (value << ConstantBits.SINGLE_BIT_VALUE) | (ulong)bit; 
            }
            return value;
        }
    }
}
