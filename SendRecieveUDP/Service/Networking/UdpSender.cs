using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Interfaces.Packet;
using SendRecieveUDP.Model.Interfaces.Udp;
using SendRecieveUDP.Model.Ro;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;

namespace SendRecieveUDP.Service.Udp
{
    public class UdpSender : IUdpSender
    {
        private readonly IPacketEncoderDecoder _packetBuilder;

        public UdpSender(IPacketEncoderDecoder packetBuilder)
        {
            _packetBuilder = packetBuilder;
        }

        public SendCsvUdpResult SendCsvUdp(string csvFile, List<IcdField> icd)
        {
            string[] lines = File.ReadAllLines(csvFile);
            if (lines.Length < ConstantCsv.MIN_ROWS_REQUIRED)
            {
                Debug.WriteLine("CSV file does not contain enough lines.");
                return new SendCsvUdpResult(false, "CSV file does not contain enough lines."); 
            }

            string[] headers = lines[ConstantCsv.HEADER_ROW_INDEX].Split(ConstantCsv.CSV_DELIMITER);
            IEnumerable<string> onlyDataLines = lines.Skip(ConstantCsv.DATA_START_ROW_INDEX);

            Dictionary<string, int> headerIndex = headers
                .Select((name, columnIndex) => new { name, columnIndex })
                .ToDictionary(column => column.name, column => column.columnIndex);

            using UdpClient udp = new UdpClient();

            foreach (string line in onlyDataLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    byte[] packet = _packetBuilder.EncodePacket(line, icd, headerIndex);
                    udp.Send(packet, packet.Length, ConstantNetwork.LOOPBACK_ADDRESS, ConstantNetwork.UDP_PORT);
                }
            }
            return new SendCsvUdpResult(true, " Send CSV successfully");
        }
    }
}
