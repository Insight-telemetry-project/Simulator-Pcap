using Microsoft.Extensions.DependencyInjection;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Model.Interfaces.Csv;
using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Interfaces.Packet;
using SendRecieveUDP.Model.Interfaces.Udp;
using SendRecieveUDP.Service.BitManipulation;
using SendRecieveUDP.Service.Csv;
using SendRecieveUDP.Service.Packet;
using SendRecieveUDP.Service.Udp;
using System.Text.Json;
using SendRecieveUDP.Common.Constant;   

namespace SendRecieveUDP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string icdJson = File.ReadAllText("icd.json");
            List<IcdField> icd = JsonSerializer.Deserialize<List<IcdField>>(icdJson);

            var services = new ServiceCollection();
            services.AddScoped<ICsvCleaner, CsvCleaner>();
            services.AddScoped<IUdpSender, UdpSender>();
            services.AddScoped<IUdpReceiver, UdpReceiver>();
            services.AddScoped<IBitEncoder, BitEncoder>();
            services.AddScoped<IPacketBuilder, PacketBuilder>();

            using var provider = services.BuildServiceProvider();


            var cancellationToken = new CancellationTokenSource();
            var reciever = provider.GetRequiredService<IUdpReceiver>();

            Task.Run(() => reciever.ReceiveUDP(icd, cancellationToken.Token));
            cancellationToken.CancelAfter(TimeSpan.FromSeconds(ConstantTime.MINUTE));




            var cleaner = provider.GetRequiredService<ICsvCleaner>();
            cleaner.Run("5ROW.csv", "Longest_Master_23517_clean.csv");

            var sender = provider.GetRequiredService<IUdpSender>();
            sender.SendCsv("Longest_Master_23517_clean.csv", icd);


            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}
