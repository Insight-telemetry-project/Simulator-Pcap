using Microsoft.Extensions.DependencyInjection;
using SendRecieveUDP.Model;
using SendRecieveUDP.Service;
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
            using var provider = services.BuildServiceProvider();

            var reciever = provider.GetRequiredService<IRecieve>();

            Task.Run(() => reciever.ReceiveUDP(icd));





            var cleaner = provider.GetRequiredService<ICsvCleaner>();
            cleaner.Run("5ROW.csv", "Longest_Master_23517_clean.csv");

            var sender = provider.GetRequiredService<ISend>();
            sender.SendCsv("Longest_Master_23517_clean.csv", icd);


            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}
