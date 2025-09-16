using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Interfaces.Packet;
using SendRecieveUDP.Model.Interfaces.Udp;
using System;
using System.Net;
using System.Net.Sockets;

namespace SendRecieveUDP.Service.Udp
{
    internal class UdpReceiver: IUdpReceiver
    {
        private readonly IPacketBuilder _packetBuilder;

        public UdpReceiver(IPacketBuilder packetBuilder)
        {
            _packetBuilder = packetBuilder;
        }
        public void ReceiveUDP(List<IcdField> icd, CancellationToken token)
        {
            using var usp = new UdpClient(ConstantNetwork.UDP_PORT);
            Console.WriteLine($"Listening on port {ConstantNetwork.UDP_PORT}...");

            while (!token.IsCancellationRequested)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = usp.Receive(ref remoteEP);

                Console.WriteLine("Received packet:");
                _packetBuilder.DecodePacket(data, icd);
            }
        }

        

    }
}
