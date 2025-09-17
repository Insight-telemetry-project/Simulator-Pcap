using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.Csv;
using SendRecieveUDP.Model.Ro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendRecieveUDP.Service.Csv
{
    public class CsvFormatter : ICsvFormatter
    {
        public FunctionResult Format(string inputFile, string outputFile)
        {
            if (!File.Exists(inputFile))
            {
                return new FunctionResult(false, $"File {inputFile} not found!"); ;
            }

            string[] lines = File.ReadAllLines(inputFile);
            if (lines.Length == ConstantCsv.EMPTY_ROW_COUNT)
            {
                return new FunctionResult(false, $"File {inputFile} CSV is empty!"); ;
            }

            using StreamWriter streamWriter = new StreamWriter(outputFile);
            streamWriter.WriteLine(lines[ConstantCsv.HEADER_ROW_INDEX]);

            for (int rowIndex = ConstantCsv.DATA_START_ROW_INDEX; rowIndex < lines.Length; rowIndex++)
            {
                string[] columns = lines[rowIndex].Split(ConstantCsv.CSV_DELIMITER);


                for (int column = ConstantCsv.FIRST_COLUMN_INDEX; column < columns.Length; column++)
                {
                    // Remove "c_" prefix from cluster columns "c_123" -> "123"
                    if (columns[column].StartsWith(ConstantCsv.CLUSTER_PREFIX))
                        columns[column] = columns[column].Substring(ConstantCsv.CLUSTER_PREFIX_LENGTH);
                }

                streamWriter.WriteLine(string.Join(ConstantCsv.CSV_DELIMITER, columns));
            }

            return new FunctionResult(true, $" Clean CSV saved to {outputFile}");
        }
    }
}
