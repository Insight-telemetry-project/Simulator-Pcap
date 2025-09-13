using SendRecieveUDP.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using SendRecieveUDP.Common.Constant;

namespace SendRecieveUDP.Service
{
    internal static class Send
    {
        public static void SendCsv(string csvFile, List<IcdField> icd)
        {
            var lines = File.ReadAllLines(csvFile);
            if (lines.Length < ConstantSend.ONLY_TITLES) return;

            var headers = lines[ConstantSend.TITLE_ROW].Split(',');
            var dataLines = lines.Skip(1);

            var headerIndex = headers
                .Select((name, idx) => new { name, idx })
                .ToDictionary(x => x.name, x => x.idx, StringComparer.Ordinal);

            using var udp = new UdpClient();

            foreach (var line in dataLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                byte[] packet = BuildPacket(line, icd, headerIndex);
                udp.Send(packet, packet.Length, ConstantSend.LOCALHOST, ConstantSend.PORT);
            }
        }

        private static byte[] BuildPacket(string csvLine, List<IcdField> icd, Dictionary<string, int> headerIndex)
        {
            

            int lastBit = icd.Max(field => field.BitOffset + field.SizeBits);
            int totalBytes = (lastBit + ConstantSend.ROUND_TO_BYTE) / ConstantSend.BITS_IN_BYTE;
            byte[] packet = new byte[totalBytes];

            var values = csvLine.Split(',');

            foreach (var field in icd)
            {
                if (!headerIndex.ContainsKey(field.Name))
                    continue;

                int colIndex = headerIndex[field.Name];
                if (colIndex < 0 || colIndex >= values.Length) continue;

                double actual = double.Parse(values[colIndex], CultureInfo.InvariantCulture);
                double scale = field.Scale;
                double shifted;
                if (field.Min < 0)
                {
                    double value = Math.Round(actual / scale);
                    double valueMin = Math.Round(field.Min / scale);
                    shifted = value - valueMin;
                }
                else
                {
                    shifted = Math.Round(actual / scale);
                }

                ulong finalValue = (ulong)shifted;
                WriteBits(packet, field.BitOffset, field.SizeBits, finalValue);
            }

            return packet;
        }

        private static void WriteBits(byte[] buffer, int bitOffset, int bitCount, ulong value)
        {
            for (int i = 0; i < bitCount; i++)
            {
                int bitVal = (int)((value >> i) & 1UL);
                int byteIdx = (bitOffset + i) / ConstantSend.BITS_IN_BYTE;
                int bitIdx = (bitOffset + i) % ConstantSend.BITS_IN_BYTE;

                if (bitVal == 1)
                    buffer[byteIdx] |= (byte)(1 << bitIdx);
                else
                    buffer[byteIdx] &= (byte)~(1 << bitIdx);
            }
        }
    }
}
