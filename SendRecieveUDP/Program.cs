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

namespace SendRecieveUDP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string icdJson = File.ReadAllText("icd.json");
            List<IcdField> icd = JsonSerializer.Deserialize<List<IcdField>>(icdJson);

            var services = new ServiceCollection();
            services.AddScoped<ICsvCleaner, CleanCsv>();
            services.AddScoped<ISend, Send>();
            services.AddScoped<IRecieve, Recieve>();
            services.AddScoped<IBitManipulator, BitManipulator>();
            services.AddScoped<IPacketBuilder, PacketBuilder>();

            using var provider = services.BuildServiceProvider();


            var cancellationToken = new CancellationTokenSource();
            var reciever = provider.GetRequiredService<IRecieve>();

            Task.Run(() => reciever.ReceiveUDP(icd, cancellationToken.Token));
            cancellationToken.CancelAfter(TimeSpan.FromSeconds(30));




            var cleaner = provider.GetRequiredService<ICsvCleaner>();
            cleaner.Run("5ROW.csv", "Longest_Master_23517_clean.csv");

            var sender = provider.GetRequiredService<ISend>();
            sender.SendCsv("Longest_Master_23517_clean.csv", icd);


            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}
