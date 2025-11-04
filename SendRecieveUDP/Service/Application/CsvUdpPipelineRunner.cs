using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.Csv;
using SendRecieveUDP.Model.Interfaces.Icd;
using SendRecieveUDP.Model.Interfaces.Udp;
using SendRecieveUDP.Model.Ro;
using System.Diagnostics;
using System.Text.Json;

namespace SendRecieveUDP.Service.Application
{
    public class CsvUdpPipelineRunner
    {
        private readonly IUdpReceiver _receiver;
        private readonly IUdpSender _sender;
        private readonly ICsvFormatter _csvFormatter;
        const double secondsInMinute = TimeSpan.TicksPerMinute / (double)TimeSpan.TicksPerSecond;


        public CsvUdpPipelineRunner(IUdpReceiver receiver, IUdpSender sender, ICsvFormatter csvFormatter)
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

            SendCsvUdpResult formatResult = _csvFormatter.Format(ConstantCsv.FLIGHT_FILE, ConstantCsv.PROCESSED_FILE);
            if (formatResult.Success)
            {
                _sender.SendCsvUdp(ConstantCsv.PROCESSED_FILE, icd);
            }
            else
            {
                Debug.WriteLine($"CSV formatting failed: {formatResult.Message}");
            }
            


        }
    }
}
