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
            var icd = JsonSerializer.Deserialize<List<IcdField>>(icdJson);

            Thread recieverThread = new Thread(() =>
            {
                Service.Recieve.ReceiveUDP(icd);
            });

            recieverThread.IsBackground = true;
            recieverThread.Start();
            Thread.Sleep(500);





            CleanCsv.Run("5ROW.csv", "Longest_Master_23517_clean.csv");


            Send.SendCSV("Longest_Master_23517_clean.csv", icd);



            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}
