using SendRecieveUDP.Common.Constant;
using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Interfaces.Packet;
using SendRecieveUDP.Model.Interfaces.Udp;
using System.Globalization;
using System.Net.Sockets;

namespace SendRecieveUDP.Service.Udp
{
    public class UdpSender : IUdpSender
    {
        private readonly IPacketBuilder _packetBuilder;

        public UdpSender(IPacketBuilder packetBuilder)
        {
            _packetBuilder = packetBuilder;
        }

        public void SendCsv(string csvFile, List<IcdField> icd)
        {
            string[] lines = File.ReadAllLines(csvFile);
            if (lines.Length < ConstantCsv.ONLY_TITLES)
            {
                Console.WriteLine("CSV file does not contain enough lines.");
                return;
            }

            string[] headers = lines[ConstantCsv.TITLE_ROW].Split(ConstantCsv.CSV_DELIMITER);
            var dataLines = lines.Skip(ConstantCsv.ONE_LINE);

            var headerIndex = headers
                .Select((name, idx) => new { name, idx })
                .ToDictionary(column => column.name, column => column.idx, StringComparer.Ordinal);

            using UdpClient udp = new UdpClient();

            foreach (string line in dataLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    byte[] packet = _packetBuilder.BuildPacket(line, icd, headerIndex);
                    udp.Send(packet, packet.Length, ConstantNetwork.LOCALHOST, ConstantNetwork.PORT);
                }
            }
        }
    }
}
