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
            if (lines.Length < 2)
            {
                Console.WriteLine("CSV is empty or has no data rows.");
                return;
            }

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
                Console.WriteLine($"Sent packet of {packet.Length} bytes");
            }
        }

        private static byte[] BuildPacket(string csvLine, List<IcdField> icd, Dictionary<string, int> headerIndex)
        {
            int totalSize = icd.Max(f => f.ByteLocation + (f.SizeBits / 8));
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
                {
                    throw new InvalidOperationException(
                        $"ICD overflow for '{field.Name}': offset={field.ByteLocation}, size={sizeInBytes}, packetLen={packet.Length}");
                }

                Buffer.BlockCopy(bytes, 0, packet, field.ByteLocation, sizeInBytes);
            }

            return packet;
        }

        private static byte[] EncodeField(string strVal, IcdField field)
        {
            int sizeInBytes = field.SizeBits / ConstantSend.BITS_IN_BYTE;

            if (field.Type == "float")
            {
                if (!float.TryParse(strVal, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float fval))
                    fval = 0f;

                return BitConverter.GetBytes(fval);
            }

            if (!int.TryParse(strVal, NumberStyles.Integer, CultureInfo.InvariantCulture, out int ival))
                ival = 0;

            if (sizeInBytes == ConstantSend.BYTE_SIZE)
            {
                if (field.Min < 0) 
                    return new byte[] { unchecked((byte)(sbyte)ival) };
                else               
                    return new byte[] { (byte)ival };
            }
            else if (sizeInBytes == ConstantSend.SHORT_SIZE)
            {
                return BitConverter.GetBytes((short)ival);
            }
            else if (sizeInBytes == ConstantSend.INT_SIZE)
            {
                return BitConverter.GetBytes(ival);
            }
            else
            {
                throw new NotSupportedException($"Unsupported int SizeBits={field.SizeBits} for '{field.Name}'");
            }
        }

    }
}
