using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Model
{
    public interface ISend
    {
        void SendCsv(string csvFile, List<IcdField> icd);
    }
}
