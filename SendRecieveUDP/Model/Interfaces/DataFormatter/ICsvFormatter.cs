using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Model.Interfaces.Csv
{
    public interface ICsvFormatter
    {
        void Format(string inputFile, string outputFile);
    }
}
