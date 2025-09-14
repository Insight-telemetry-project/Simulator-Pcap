using SendRecieveUDP.Common.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Interfaces.Udp;
using System;
using System.Net;
using System.Net.Sockets;

namespace SendRecieveUDP.Service.Udp
{
    internal class Recieve: IRecieve
    {
        private readonly IBitManipulator _bitManipulator;

        public Recieve(IBitManipulator bitManipulator)
        {
            _bitManipulator = bitManipulator;
        }
        public void ReceiveUDP(List<IcdField> icd, CancellationToken token)
        {
            using var usp = new UdpClient(ConstantNetwork.PORT);
            Console.WriteLine("Listening on port 5000...");

            while (!token.IsCancellationRequested)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, ConstantBits.ZERO);
                byte[] data = usp.Receive(ref remoteEP);

                Console.WriteLine("Received packet:");
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

                        Console.WriteLine($"  {field.Name}: {actual} {field.Units}  [raw={raw}]");
                    }
                    else
                    {
                        Console.WriteLine($"  {field.Name}: out of bounds (bitOffset={field.BitOffset}, sizeBits={field.SizeBits}, lenBits={data.Length * ConstantBits.BITS_IN_BYTE})");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
