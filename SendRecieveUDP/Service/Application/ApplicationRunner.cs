using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.Csv;
using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Interfaces.Udp;
using SendRecieveUDP.Model.Ro;
using System.Diagnostics;
using System.Text.Json;

namespace SendRecieveUDP.Service.Application
{
    public class ApplicationRunner
    {
        private readonly IUdpReceiver _receiver;
        private readonly IUdpSender _sender;
        private readonly ICsvFormatter _csvFormatter;

        public ApplicationRunner(IUdpReceiver receiver, IUdpSender sender, ICsvFormatter csvFormatter)
        {
            _receiver = receiver;
            _sender = sender;
            _csvFormatter = csvFormatter;
        }

        public void Run()
        {
            string icdJson = File.ReadAllText("icd.json");
            List<IcdField> icd = JsonSerializer.Deserialize<List<IcdField>>(icdJson);

            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            Task.Run(() => _receiver.ReceiveUDP(icd, cancellationToken.Token));
            cancellationToken.CancelAfter(TimeSpan.FromSeconds(ConstantTime.SECONDS_IN_MINUTE));

            FunctionResult formatResult = _csvFormatter.Format("5ROW.csv", "Longest_Master_23517_clean.csv");
            if (formatResult.Success)
            {
                _sender.SendCsvUdp("Longest_Master_23517_clean.csv", icd);
            }
            else
            {
                Debug.WriteLine($"CSV formatting failed: {formatResult.Message}");
            }

            
        }
    }
}
