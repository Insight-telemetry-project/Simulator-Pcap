using SendRecieveUDP.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using SendRecieveUDP.Common.Constant;

namespace SendRecieveUDP.Service
{
    public class Send : ISend
    {
        private readonly IBitManipulator _bitManipulator;

        public Send(IBitManipulator bitManipulator)
        {
            _bitManipulator = bitManipulator;
        }

        public void SendCsv(string csvFile, List<IcdField> icd)
        {
            string[] lines = File.ReadAllLines(csvFile);
            if (lines.Length < ConstantSend.ONLY_TITLES) 
            {
                Console.WriteLine("CSV file does not contain enough lines.");
                return;
            }

            string[] headers = lines[ConstantSend.TITLE_ROW].Split(',');
            var dataLines = lines.Skip(1);

            var headerIndex = headers
                .Select((name, idx) => new { name, idx })
                .ToDictionary(x => x.name, x => x.idx, StringComparer.Ordinal);

            using var udp = new UdpClient();

            foreach (string line in dataLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    byte[] packet = BuildPacket(line, icd, headerIndex);
                    udp.Send(packet, packet.Length, ConstantSend.LOCALHOST, ConstantSend.PORT);
                }
            }
        }

        private byte[] BuildPacket(string csvLine, List<IcdField> icd, Dictionary<string, int> headerIndex)
        {


            int lastBit = icd.Max(field => field.BitOffset + field.SizeBits);
            int totalBytes = (lastBit + ConstantSend.ROUND_TO_BYTE) / ConstantSend.BITS_IN_BYTE;
            byte[] packet = new byte[totalBytes];

            string[] csvColumns = csvLine.Split(',');

            foreach (IcdField icdField in icd)
            {
                if (headerIndex.TryGetValue(icdField.Name, out int colIndex)
                && colIndex < csvColumns.Length)
                {
                    double rawValue = double.Parse(csvColumns[colIndex], CultureInfo.InvariantCulture);
                    double scaleFactor = icdField.Scale;
                    double shifted;
                    if (icdField.Min < 0)
                    {
                        double value = Math.Round(rawValue / scaleFactor);
                        double valueMin = Math.Round(icdField.Min / scaleFactor);
                        shifted = value - valueMin;
                    }
                    else
                    {
                        shifted = Math.Round(rawValue / scaleFactor);
                    }

                    ulong finalValue = (ulong)shifted;
                    WriteBits(packet, icdField.BitOffset, icdField.SizeBits, finalValue);
                }
            }

            return packet;
        }

        private void WriteBits(byte[] buffer, int bitOffset, int bitCount, ulong value)
        {
            _bitManipulator.WriteBits(buffer, bitOffset, bitCount, value);
        }
    }
}
