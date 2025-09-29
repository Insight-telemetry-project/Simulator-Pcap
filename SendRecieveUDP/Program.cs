using Microsoft.Extensions.DependencyInjection;
using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Model.Interfaces.Csv;
using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Interfaces.Packet;
using SendRecieveUDP.Model.Interfaces.Udp;
using SendRecieveUDP.Model.Ro;
using SendRecieveUDP.Service.Application;
using SendRecieveUDP.Service.BitManipulation;
using SendRecieveUDP.Service.Csv;
using SendRecieveUDP.Service.Packet;
using SendRecieveUDP.Service.Udp;
using System.Diagnostics;
using System.Text.Json;

namespace SendRecieveUDP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IUdpReceiver, UdpReceiver>();
            services.AddSingleton<IUdpSender, UdpSender>();
            services.AddSingleton<IPacketEncoderDecoder, PacketEncoderDecoder>();
            services.AddSingleton<IBitEncoder, BitEncoder>();
            services.AddSingleton<ICsvFormatter, CsvFormatter>();
            services.AddSingleton<CsvUdpPipelineRunner>();

            using ServiceProvider provider = services.BuildServiceProvider();

            


            CsvUdpPipelineRunner app = provider.GetRequiredService<CsvUdpPipelineRunner>();
            app.Run();

        }
    }
}
