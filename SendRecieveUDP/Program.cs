using SendRecieveUDP.Model;
using SendRecieveUDP.Service;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
namespace SendRecieveUDP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string icdJson = File.ReadAllText("icd.json");
            List<IcdField> icd = JsonSerializer.Deserialize<List<IcdField>>(icdJson);

            

            Task.Run(() => Service.Recieve.ReceiveUDP(icd));

            CleanCsv.Run("5ROW.csv", "Longest_Master_23517_clean.csv");


            Send.SendCsv("Longest_Master_23517_clean.csv", icd);



            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}
