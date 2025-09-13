using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Common.Constant
{
    internal class ConstantSend
    {
        public const int PORT = 5000;
        public const string LOCALHOST = "127.0.0.1";
        public const char CSV_DELIMITER = ',';

        public const int BYTE_SIZE = 1;   
        public const int SHORT_SIZE = 2; 
        public const int INT_SIZE = 4;   
        public const int FLOAT_SIZE = 4;
        public const int BITS_IN_BYTE = 8;
        public const int ONLY_TITLES = 2;
        public const int TITLE_ROW = 0;
        public const int ROUND_TO_BYTE = 7;
    }
}
