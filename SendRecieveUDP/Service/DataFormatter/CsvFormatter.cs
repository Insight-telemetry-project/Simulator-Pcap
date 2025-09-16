using SendRecieveUDP.Model.Interfaces.Csv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendRecieveUDP.Model.Constant;

namespace SendRecieveUDP.Service.Csv
{
    public class CsvFormatter : ICsvFormatter
    {
        public void Format(string inputFile, string outputFile)
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($" File {inputFile} not found!");
                return;
            }

            string[] lines = File.ReadAllLines(inputFile);
            if (lines.Length == ConstantCsv.EMPTY_ROW_COUNT)
            {
                Console.WriteLine(" CSV is empty!");
                return;
            }

            using var streamWriter = new StreamWriter(outputFile);
            streamWriter.WriteLine(lines[ConstantCsv.HEADER_ROW_INDEX]);

            for (int rowIndex = ConstantCsv.DATA_START_ROW_INDEX; rowIndex < lines.Length; rowIndex++)
            {
                string[] parts = lines[rowIndex].Split(ConstantCsv.CSV_DELIMITER);

                for (int column = ConstantCsv.EMPTY_ROW_COUNT; column < parts.Length; column++)
                {
                    if (parts[column].StartsWith(ConstantCsv.CLUSTER_PREFIX))
                        parts[column] = parts[column].Substring(ConstantCsv.CLUSTER_PREFIX_LENGTH);
                }

                streamWriter.WriteLine(string.Join(ConstantCsv.CSV_DELIMITER, parts));
            }

            Console.WriteLine($" Clean CSV saved to {outputFile}");
        }
    }
}
