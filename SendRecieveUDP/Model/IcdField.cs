using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Model
{
    internal class IcdField
    {
        public string Name { get; set; }
        public string Units { get; set; }
        public int BitOffset { get; set; }
        public int SizeBits { get; set; }
        public double Scale { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }
}
