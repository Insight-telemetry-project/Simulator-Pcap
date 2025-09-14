using SendRecieveUDP.Common.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;

namespace SendRecieveUDP.Service.BitManipulation
{
    public class BitManipulator : IBitManipulator
    {
        public void WriteBits(byte[] buffer, int bitOffset, int bitCount, ulong value)
        {
            for (int indexInByte = 0; indexInByte < bitCount; indexInByte++)
            {
                // נחשב לאיזה בייט ולביט בתוך הבייט לכתוב
                int byteIdx = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIdx = 7 - ((bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE); // MSB-first

                // הביט מתוך value – לוקחים משמאל לימין
                int bitVal = (int)((value >> (bitCount - 1 - indexInByte)) & 1);

                if (bitVal == 1)
                    buffer[byteIdx] |= (byte)(1 << bitIdx);
                else
                    buffer[byteIdx] &= (byte)~(1 << bitIdx);
            }
        }

        public ulong ReadBits(byte[] buffer, int bitOffset, int bitCount)
        {
            ulong value = 0;
            for (int indexInByte = 0; indexInByte < bitCount; indexInByte++)
            {
                int byteIdx = (bitOffset + indexInByte) / ConstantBits.BITS_IN_BYTE;
                int bitIdx = 7 - ((bitOffset + indexInByte) % ConstantBits.BITS_IN_BYTE); // MSB-first

                int bit = (buffer[byteIdx] >> bitIdx) & 1;
                value = (value << 1) | (ulong)bit; // מכניס משמאל לימין
            }
            return value;
        }
    }
}
