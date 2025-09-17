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
            int totalBytes = (lastBit + ConstantBits.MAX_BIT_INDEX_IN_BYTE) / ConstantBits.BITS_IN_BYTE;
            byte[] packet = new byte[totalBytes];

            string[] csvColumns = csvLine.Split(ConstantCsv.CSV_DELIMITER);

            EncodeFieldsIntoPacket(packet, csvColumns, icd, headerIndex);

            return packet;
        }

        private void EncodeFieldsIntoPacket(byte[] packet, string[] csvColumns, List<IcdField> icd, Dictionary<string, int> headerIndex)
        {
            foreach (IcdField icdField in icd)
            {
                if (headerIndex.TryGetValue(icdField.Name, out int colIndex)
                    && colIndex < csvColumns.Length)
                {
                    double rawValue = double.Parse(csvColumns[colIndex], CultureInfo.InvariantCulture);
                    double scaleFactor = icdField.Scale;

                    double shifted;
                    if (icdField.Min < ConstantCsv.EMPTY_ROW_COUNT)
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
                    _bitManipulator.WriteBits(packet, icdField.BitOffset, icdField.SizeBits, finalValue);
                }
            }
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
                   Debug.WriteLine($"  {field.Name}: out of bounds (bitOffset={field.BitOffset}, sizeBits={field.SizeBits}, lenBits={data.Length * ConstantBits.BITS_IN_BYTE})");
                }
            }
        }
    }


}
