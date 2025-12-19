using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.Csv;
using SendRecieveUDP.Model.Ro;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SendRecieveUDP.Service.Csv
{
    public class CsvFormatter : ICsvFormatter
    {
        public SendCsvUdpResult Format(string inputFile, string outputFile)
        {
            if (!File.Exists(inputFile))
                return new SendCsvUdpResult(false, $"File {inputFile} not found!");

            string[] lines = File.ReadAllLines(inputFile);
            if (lines.Length == ConstantCsv.EMPTY_ROW_COUNT)
                return new SendCsvUdpResult(false, $"File {inputFile} CSV is empty!");

            long baseEpoch = GetBaseEpoch();

            string[] headers = lines[ConstantCsv.HEADER_ROW_INDEX].Split(ConstantCsv.CSV_DELIMITER);
            int timestepIndex = Array.IndexOf(headers, "timestep");

            using StreamWriter writer = new StreamWriter(outputFile);
            writer.WriteLine(lines[ConstantCsv.HEADER_ROW_INDEX]);

            foreach (string line in lines.Skip(ConstantCsv.DATA_START_ROW_INDEX))
            {
                string[] columns = line.Split(ConstantCsv.CSV_DELIMITER);
                ConvertTimestepToEpoch(columns, timestepIndex, baseEpoch);
                CleanClusterPrefixes(columns);
                writer.WriteLine(string.Join(ConstantCsv.CSV_DELIMITER, columns));
            }

            return new SendCsvUdpResult(true, $"Clean CSV saved to {outputFile}");
        }

        private long GetBaseEpoch()
        {
            DateTime epochStart = DateTime.Now.Date;
            return new DateTimeOffset(epochStart).ToUnixTimeSeconds();
        }

        private void ConvertTimestepToEpoch(string[] columns, int timestepIndex, long baseEpoch)
        {
            if (double.TryParse(columns[timestepIndex], NumberStyles.Float, CultureInfo.InvariantCulture, out double secondsFromStart))
            {
                long epochValue = baseEpoch + (long)secondsFromStart;
                columns[timestepIndex] = epochValue.ToString(CultureInfo.InvariantCulture);
                Console.WriteLine($"Converted timestep  {epochValue}");
            }
        }

        private void CleanClusterPrefixes(string[] columns)
        {
            for (int i = ConstantCsv.FIRST_COLUMN_INDEX; i < columns.Length; i++)
            {
                if (columns[i].StartsWith(ConstantCsv.CLUSTER_PREFIX))
                    columns[i] = columns[i].Substring(ConstantCsv.CLUSTER_PREFIX.Length);
            }
        }
    }
}
