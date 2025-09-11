using SendRecieveUDP.Common.Constant;
using SendRecieveUDP.Model;
using System;
using System.Net;
using System.Net.Sockets;

namespace SendRecieveUDP.Service
{
    internal static class Recieve
    {
        public static void ReceiveUDP(List<IcdField> icd)
        {
            using var usp = new UdpClient(ConstantSend.PORT);
            Console.WriteLine("Listening on port 5000...");

            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = usp.Receive(ref remoteEP);

                Console.WriteLine("Received packet:");
                for (int i = 0; i < icd.Count; i++)
                {
                    var field = icd[i];
                    int sizeInBytes = field.SizeBits / 8;

                    if (field.ByteLocation < 0 || field.ByteLocation + sizeInBytes > data.Length)
                    {
                        Console.WriteLine($"  {field.Name}: out of bounds (offset={field.ByteLocation}, size={sizeInBytes}, len={data.Length})");
                        continue;
                    }

                    long raw = DecodeRaw(data, field.ByteLocation, sizeInBytes, field.Min < 0);
                    double actual = raw * (field.Scale == 0 ? 1.0 : field.Scale);

                    Console.WriteLine($"  {field.Name}: {actual} {field.Units}  [raw={raw}]");
                }

                Console.WriteLine();
            }
        }

        private static long DecodeRaw(byte[] buffer, int offset, int sizeInBytes, bool isSigned)
        {
            switch (sizeInBytes)
            {
                case 1:
                    return isSigned ? (sbyte)buffer[offset] : buffer[offset];

                case 2:
                    return isSigned
                        ? BitConverter.ToInt16(buffer, offset)
                        : BitConverter.ToUInt16(buffer, offset);

                case 4:
                    return isSigned
                        ? BitConverter.ToInt32(buffer, offset)
                        : BitConverter.ToUInt32(buffer, offset);

                default:
                    throw new NotSupportedException($"Unsupported field size: {sizeInBytes * ConstantSend.BITS_IN_BYTE} bits");
            }
        }
    }
}
