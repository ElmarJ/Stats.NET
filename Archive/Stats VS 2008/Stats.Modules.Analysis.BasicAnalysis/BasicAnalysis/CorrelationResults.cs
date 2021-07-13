using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Stats.Core.Analysis;
using Stats.Core.Data;
using Stats.Core.Results;
using System.ComponentModel.Composition;

namespace Stats.Modules.Analysis
{
    public class CorrelationResults: Results
    {
        CorrelationCollection correlations;

        internal CorrelationResults(CorrelationCollection correlations)
        {
            this.correlations = correlations;
        }

        public CorrelationResults()
        {
        }

        public CorrelationCollection Correlations
        {
            get { return correlations; }
            set { correlations = value; }
        }

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
            foreach (KeyValuePair<IVariable, Dictionary<IVariable, double>> kvp in this.correlations)
            {
                table.Rows[rowId].Header = kvp.Key.Name;
                table.Columns[rowId].Header = kvp.Key.Name;

                int colId = 0;
                foreach (KeyValuePair<IVariable, double> kvp2 in kvp.Value)
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
