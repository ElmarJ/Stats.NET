using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpssLib.FileParser;
using System.IO;

namespace SpssLib.SpssDataset
{
    public class SpssDataset
    {
        public VariablesCollection Variables { get; private set; }
        public RecordCollection Records { get; private set; }

        internal double SysmisValue { get; private set; }
        internal SavFileParser parser;

        public SpssDataset()
        {
            this.Variables = new VariablesCollection();
            this.Records = new RecordCollection();
        }

        public SpssDataset(SavFileParser parser)
            : this()
        {
            this.parser = parser;
            foreach (var variable in parser.Variables)
            {
                this.Variables.Add(variable);
            }
        }

        public SpssLib.DataReader.SpssDataReader DataReader
        {
            get
            {
                if (this.parser == null)
                    throw new InvalidOperationException();
                return this.parser.GetDataReader();
            }
        }

        public SpssDataset(Stream fileStream)
            : this(new SavFileParser(fileStream))
        {
        }
    }
}
