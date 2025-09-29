using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Model.Ro
{
    public class SendCsvUdpResult
    {
        public Boolean Success { get; }
        public string Message { get;}

        public SendCsvUdpResult(Boolean success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
