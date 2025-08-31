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
        public int ByteLocation { get; set; }
        public int SizeBits { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public string Type { get; set; }

    }
}
