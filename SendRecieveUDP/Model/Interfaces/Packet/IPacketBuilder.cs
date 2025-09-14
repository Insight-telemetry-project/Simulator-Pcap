using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendRecieveUDP.Model.Interfaces.Icd;
namespace SendRecieveUDP.Model.Interfaces.Packet
{
    public interface IPacketBuilder
    {
        byte[] BuildPacket(string csvLine, List<IcdField> icd, Dictionary<string, int> headerIndex);

        void DecodePacket(byte[] data, List<IcdField> icd);

    }
}
