using SendRecieveUDP.Model.Ro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Model.Interfaces.Csv
{
    public interface ICsvFormatter
    {
        FunctionResult Format(string inputFile, string outputFile);
    }
}
