using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Stats.Core.Data;
using Stats.Core.Data.Observations;

namespace Stats.Interoperability.CSV
{
    public class CsvDataMatrixImporter: IDataMatrixImporter
    {
        public Core.Data.IDataMatrix Import(System.IO.Stream importStream)
        {
            var reader = new StreamReader(importStream);
            IDataMatrix matrix = new InMemoryDataMatrix();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                var record = line.Split(',');


                var values =
                    from value in record
                    select (value);
                var rec = new Record(matrix, (string[])values);

                int i = 0;
                matrix.Records.Add(rec);

            }
            return matrix;
        }
    }
}
