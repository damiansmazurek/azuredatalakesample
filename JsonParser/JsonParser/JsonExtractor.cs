using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace JsonParser
{
 
    [SqlUserDefinedExtractor(AtomicFileProcessing = true)]
    public class JsonExtractor : IExtractor
    {
        public static JsonExtractor ExtractJSON()
        {
            return new JsonExtractor();
        }

        public override IEnumerable<IRow> Extract(IUnstructuredReader input, IUpdatableRow output)
        {
            string line;
            //Read the input line by line
            foreach (Stream current in input.Split(Encoding.UTF8.GetBytes("\r\n")))
            {
                using (System.IO.StreamReader streamReader = new StreamReader(current, Encoding.UTF8))
                {
                    line = streamReader.ReadToEnd().Trim();
                    var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataDTO>(line);
                    string csvValues = jsonData.Concat;
                    var separeteColumns = csvValues.Split(';');
                    output.Set<string>(0, separeteColumns[0]);
                    output.Set<string>(1, separeteColumns[1]);
                    output.Set<double>(2, double.Parse(separeteColumns[2], CultureInfo.InvariantCulture));
                    output.Set<DateTime>(3, DateTime.Parse(separeteColumns[3]));
                    yield return output.AsReadOnly();
                }
            }
        }
    }
}