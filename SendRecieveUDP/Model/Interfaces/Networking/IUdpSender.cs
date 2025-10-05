using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Ro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Model.Interfaces.Udp
{
    public interface IUdpSender
    {
        SendCsvUdpResult SendCsvUdp(string csvFile, List<IcdField> icd);
    }
}
