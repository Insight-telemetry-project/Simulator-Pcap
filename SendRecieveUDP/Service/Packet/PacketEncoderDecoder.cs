using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Interfaces.Packet;
using System.Diagnostics;
using System.Globalization;

namespace SendRecieveUDP.Service.Packet
{
    public class PacketEncoderDecoder : IPacketEncoderDecoder
    {
        private readonly IBitEncoder _bitManipulator;

        public PacketEncoderDecoder(IBitEncoder bitManipulator)
        {
            _bitManipulator = bitManipulator;
        }

        public byte[] EncodePacket(string csvLine, List<IcdField> icd, Dictionary<string, int> headerIndex)
        {
            int lastBit = icd.Max(field => field.BitOffset + field.SizeBits);
            int totalBytes = (lastBit + ConstantBits.BITS_IN_BYTE - 1) / ConstantBits.BITS_IN_BYTE;
            byte[] packet = new byte[totalBytes];

            string[] csvColumns = csvLine.Split(ConstantCsv.CSV_DELIMITER);

            EncodeFieldsIntoPacket(packet, csvColumns, icd, headerIndex);

            return packet;
        }


        private void EncodeFieldsIntoPacket(byte[] packet, string[] csvColumns, List<IcdField> icd, Dictionary<string, int> headerIndex)
        {
            foreach (IcdField field in icd)
            {
                if (headerIndex.TryGetValue(field.Name, out int colIndex) && colIndex < csvColumns.Length)
                {
                    double rawValue = ParseValue(csvColumns[colIndex], field.Name);
                    double scaledValue = CalculateScaledValue(field, rawValue);

                    ulong finalValue = (ulong)scaledValue;
                    _bitManipulator.WriteBits(packet, field.BitOffset, field.SizeBits, finalValue);
                }
            }
        }

        private double ParseValue(string? text, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            if (double.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                return val;

            Debug.WriteLine($"Invalid value for '{fieldName}' ('{text}') replaced with 0");
            return 0;
        }

        private double CalculateScaledValue(IcdField field, double rawValue)
        {
            double scaleFactor = field.Scale;
            if (field.Min < 0)
            {
                double value = Math.Round(rawValue / scaleFactor);
                double valueMin = Math.Round(field.Min / scaleFactor);
                return value - valueMin;
            }

            return Math.Round(rawValue / scaleFactor);
        }


        public void DecodePacket(byte[] data, List<IcdField> icd)
        {
        
            foreach (IcdField field in icd)
            {
                int lastBit = field.BitOffset + field.SizeBits;
                if (lastBit <= data.Length * ConstantBits.BITS_IN_BYTE)
                {
                    ulong value = _bitManipulator.ReadBits(data, field.BitOffset, field.SizeBits);

                    double scale = field.Scale;
                    double valueMin = Math.Round(field.Min / scale);
                    double raw = value + valueMin;
                    double actual = raw * scale;

                   Debug.WriteLine($"  {field.Name}: {actual} {field.Units}  [raw={raw}]");
                }
                else
                {
                   Console.WriteLine(field.BitOffset);
                    Console.WriteLine($"  {field.Name}: out of bounds (bitOffset={field.BitOffset}, sizeBits={field.SizeBits}, lenBits={data.Length * ConstantBits.BITS_IN_BYTE})");
                }
            }
        }
    }


}
