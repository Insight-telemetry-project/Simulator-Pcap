using Microsoft.Extensions.DependencyInjection;
using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Model.Interfaces.Csv;
using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Interfaces.Packet;
using SendRecieveUDP.Model.Interfaces.Udp;
using SendRecieveUDP.Model.Ro;
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
            string icdJson = File.ReadAllText("icd.json");
            List<IcdField> icd = JsonSerializer.Deserialize<List<IcdField>>(icdJson);

            var services = new ServiceCollection();
            services.AddSingleton<IUdpReceiver, UdpReceiver>();
            services.AddSingleton<IUdpSender, UdpSender>();
            services.AddSingleton<IPacketEncoderDecoder, PacketEncoderDecoder>();
            services.AddSingleton<IBitEncoder, BitEncoder>();
            services.AddSingleton<ICsvFormatter, CsvFormatter>();


            using ServiceProvider provider = services.BuildServiceProvider();


            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            IUdpReceiver reciever = provider.GetRequiredService<IUdpReceiver>();

            Task.Run(() => reciever.ReceiveUDP(icd, cancellationToken.Token));
            cancellationToken.CancelAfter(TimeSpan.FromSeconds(ConstantTime.SECONDS_IN_MINUTE));




            ICsvFormatter cleaner = provider.GetRequiredService<ICsvFormatter>();
            FunctionResult CsvFunctionResult = cleaner.Format("5ROW.csv", "Longest_Master_23517_clean.csv");
            if (CsvFunctionResult.Success)
            {
                IUdpSender sender = provider.GetRequiredService<IUdpSender>();
                sender.SendCsv("Longest_Master_23517_clean.csv", icd);
            }


            Debug.WriteLine("Press ENTER to exit...");
        }
    }
}
