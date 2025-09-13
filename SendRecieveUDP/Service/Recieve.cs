using SendRecieveUDP.Common.Constant;
using SendRecieveUDP.Model;
using System;
using System.Net;
using System.Net.Sockets;

namespace SendRecieveUDP.Service
{
    internal class Recieve: IRecieve
    {
        public void ReceiveUDP(List<IcdField> icd)
        {
            using var usp = new UdpClient(ConstantSend.PORT);
            Console.WriteLine("Listening on port 5000...");

            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = usp.Receive(ref remoteEP);

                Console.WriteLine("Received packet:");
                foreach (var field in icd)
                {
                    int lastBit = field.BitOffset + field.SizeBits;
                    if (lastBit > data.Length * ConstantSend.BITS_IN_BYTE)
                    {
                        Console.WriteLine($"  {field.Name}: out of bounds (bitOffset={field.BitOffset}, sizeBits={field.SizeBits}, lenBits={data.Length * 8})");
                        continue;
                    }

                    ulong value = ReadBits(data, field.BitOffset, field.SizeBits);

                    double scale = field.Scale;
                    double valueMin = Math.Round(field.Min / scale);
                    double raw = value + valueMin;
                    double actual = raw * scale;

                    Console.WriteLine($"  {field.Name}: {actual} {field.Units}  [raw={raw}]");
                }

                Console.WriteLine();
            }
        }

        private ulong ReadBits(byte[] buffer, int bitOffset, int bitCount)
        {

            
            ulong value = 0UL;
            for (int i = 0; i < bitCount; i++)
            {
                int byteIdx = (bitOffset + i) / ConstantSend.BITS_IN_BYTE;
                int bitIdx = (bitOffset + i) % ConstantSend.BITS_IN_BYTE;
                int bit = (buffer[byteIdx] >> bitIdx) & ConstantSend.BYTE_SIZE;
                value |= ((ulong)bit << i);
            }
            return value;
        }
    }
}
