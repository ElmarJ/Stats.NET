using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MathLib.Core.Analysis;
using MathLib.Core.Data;
using MathLib.Core.Results;

namespace MathLib.Modules.Analysis
{
    public class CorrelationResults: Results, IXmlSerializable
    {
        CorrelationCollection correlations;

        internal CorrelationResults(CorrelationCollection correlations)
        {
            this.correlations = correlations;
        }

        public CorrelationResults()
        {
        }

        [XmlIgnore]
        public CorrelationCollection Correlations
        {
            get { return correlations; }
            set { correlations = value; }
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach(KeyValuePair<Variable, Dictionary<Variable, double>> kvp in this.correlations)
            {
                writer.WriteStartElement("Variable");
                writer.WriteAttributeString("Name", kvp.Key.Name);
                foreach (KeyValuePair<Variable, double> kvp2 in kvp.Value)
                {
                    writer.WriteStartElement("Correlation");
                    writer.WriteAttributeString("Name", kvp2.Key.Name);
                    writer.WriteValue(kvp2.Value.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        #endregion


        public override ElementCollection Elements
        {
            get
            {
                ElementCollection elements = new ElementCollection();
                elements.Add(buildTable());

                return elements;
            }
        }

        private TableElement buildTable()
        {
            TableElement table = new TableElement();

            table.Title = "Correlaties";

            for(int i=0; i<this.correlations.Count; i++)
            {
                table.AddRow();
                table.AddColumn();
            }
            
            int rowId = 0;
            foreach (KeyValuePair<Variable, Dictionary<Variable, double>> kvp in this.correlations)
            {
                table.Rows[rowId].Header = kvp.Key.Name;
                table.Columns[rowId].Header = kvp.Key.Name;

                int colId = 0;
                foreach (KeyValuePair<Variable, double> kvp2 in kvp.Value)
                {
                    table[rowId, colId].Value = kvp2.Value;
                    colId++;
                }

                rowId++;
            }

            return table;
        }
    }
}
