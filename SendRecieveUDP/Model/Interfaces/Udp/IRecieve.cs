using SendRecieveUDP.Model.Interfaces.Icd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Model.Interfaces.Udp
{
    public interface IRecieve
    {
        void ReceiveUDP(List<IcdField> icd, CancellationToken token);
    }
}
