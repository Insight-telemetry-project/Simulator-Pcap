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
        public static void SendCSV(string csvFile, List<IcdField> icd)
        {
            var lines = File.ReadAllLines(csvFile);
            if (lines.Length < 2) return;

            var headers = lines[0].Split(',');
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
            int totalSize = icd.Max(f => f.ByteLocation + (f.SizeBits / ConstantSend.BITS_IN_BYTE));
            byte[] packet = new byte[totalSize];

            var values = csvLine.Split(',');

            foreach (var field in icd)
            {
                if (!headerIndex.TryGetValue(field.Name, out int colIndex)) continue;
                if (colIndex < 0 || colIndex >= values.Length) continue;

                string strVal = values[colIndex];
                byte[] bytes = EncodeField(strVal, field);
                int sizeInBytes = field.SizeBits / ConstantSend.BITS_IN_BYTE;

                if (field.ByteLocation < 0 || field.ByteLocation + sizeInBytes > packet.Length)
                    throw new InvalidOperationException($"ICD overflow for '{field.Name}': offset={field.ByteLocation}, size={sizeInBytes}, packetLen={packet.Length}");

                Buffer.BlockCopy(bytes, 0, packet, field.ByteLocation, sizeInBytes);
            }

            return packet;
        }

        private static byte[] EncodeField(string strVal, IcdField field)
        {
            if (!double.TryParse(strVal, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double actual))
                throw new FormatException($"Cannot parse '{field.Name}' value '{strVal}'");

            double scale = field.Scale == 0 ? 1.0 : field.Scale;
            double rawDouble = actual / scale;
            long raw = checked((long)Math.Round(rawDouble, MidpointRounding.ToEven));

            int sizeInBytes = field.SizeBits / ConstantSend.BITS_IN_BYTE;
            bool isSigned = field.Min < 0;

            return sizeInBytes switch
            {
                ConstantSend.BYTE_SIZE => isSigned
                    ? new[] { unchecked((byte)(sbyte)checked(raw)) }
                    : new[] { (byte)checked(raw) },

                ConstantSend.SHORT_SIZE => isSigned
                    ? BitConverter.GetBytes((short)checked(raw))
                    : BitConverter.GetBytes((ushort)checked(raw)),

                ConstantSend.INT_SIZE => isSigned
                    ? BitConverter.GetBytes((int)checked(raw))
                    : BitConverter.GetBytes((uint)checked(raw)),

                _ => throw new NotSupportedException($"Unsupported SizeBits={field.SizeBits} for '{field.Name}'")
            };
        }
    }
}
