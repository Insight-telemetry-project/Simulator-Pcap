using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Model
{
    public interface IBitManipulator
    {
        void WriteBits(byte[] buffer, int bitOffset, int bitCount, ulong value);
        ulong ReadBits(byte[] buffer, int bitOffset, int bitCount);
    }
}
