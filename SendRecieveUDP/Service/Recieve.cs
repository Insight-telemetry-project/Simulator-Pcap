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
            using var usp = new UdpClient(5000);
            Console.WriteLine("Listening on port 5000...");

            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = usp.Receive(ref remoteEP);

                Console.WriteLine("Received packet:");
                for (int i = 0; i < icd.Count; i++)
                {
                    int offset = icd[i].ByteLocation;
                    int sizeInBytes = icd[i].SizeBits / 8;
                    byte[] fieldBytes = new byte[sizeInBytes];
                    Buffer.BlockCopy(data, offset, fieldBytes, 0, sizeInBytes);

                    if (icd[i].Type == "float")
                    {
                        float fval = BitConverter.ToSingle(fieldBytes, 0);
                        Console.WriteLine($"  {icd[i].Name}: {fval}");
                    }
                    else if (icd[i].Type == "int")
                    {
                        if (sizeInBytes == 1)
                        {
                            if (icd[i].Min < 0)
                            {
                                sbyte sval = (sbyte)fieldBytes[0];
                                Console.WriteLine($"  {icd[i].Name}: {sval}");
                            }
                            else
                            {
                                byte bval = fieldBytes[0];
                                Console.WriteLine($"  {icd[i].Name}: {bval}");
                            }
                        }
                        else if (sizeInBytes == 2)
                        {
                            short sval = BitConverter.ToInt16(fieldBytes, 0);
                            Console.WriteLine($"  {icd[i].Name}: {sval}");
                        }
                        else if (sizeInBytes == 4)
                        {
                            int ival = BitConverter.ToInt32(fieldBytes, 0);
                            Console.WriteLine($"  {icd[i].Name}: {ival}");
                        }
                        else
                        {
                            Console.WriteLine($"  {icd[i].Name}: unsupported int size ({sizeInBytes * 8} bits)");
                        }
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
