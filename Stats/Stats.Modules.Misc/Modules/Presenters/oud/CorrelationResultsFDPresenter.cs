using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using MathLib.Core.Analysis.Presenters;
using MathLib.Core.Data;
using MathLib.Modules.Analysis;

namespace MathLib.Modules.Presenters
{
    public class CorrelationResultsFDPresenter: FlowDocumentResultPresenter<CorrelationResults>
    {
        public CorrelationResultsFDPresenter(CorrelationResults results)
            : base(results)
        {
        }

        public override FlowDocument GenerateFlowDocument()
        {
            return getFD();
        }

        private FlowDocument getFD()
        {
            FlowDocument doc = new FlowDocument();

            Table table = new Table();
            table.BorderThickness = new Thickness(2);
            table.RowGroups.Add(new TableRowGroup());

            TableRow upperRow = new TableRow();
            upperRow.Cells.Add(new TableCell());
            foreach (KeyValuePair<Variable, Dictionary<Variable, double>> varCorrelations in this.Results.Correlations)
            {
                TableCell cell = new TableCell(new Paragraph(new Run(varCorrelations.Key.ToString())));
                cell.FontWeight = FontWeights.Bold;

                upperRow.Cells.Add(cell);

            }
            table.RowGroups[0].Rows.Add(upperRow);

            foreach (KeyValuePair<Variable, Dictionary<Variable, double>> varCorrelations in this.Results.Correlations)
            {
                TableRow row = new TableRow();

                TableCell nameCell = new TableCell();
                nameCell.FontWeight = FontWeights.Bold;
                nameCell.BorderThickness = new Thickness(1);
                nameCell.Blocks.Add(new Paragraph(new Run(varCorrelations.Key.ToString())));
                row.Cells.Add(nameCell);

                foreach (KeyValuePair<Variable, double> correlation in varCorrelations.Value)
                {
                    TableCell valueCell = new TableCell();
                    valueCell.BorderThickness = new Thickness(1);

                    valueCell.Blocks.Add(new Paragraph(new Run(correlation.Value.ToString())));
                    row.Cells.Add(valueCell);
                }

                table.RowGroups[0].Rows.Add(row);
            }

            doc.Blocks.Add(table);
            ResourceDictionary rd = new ResourceDictionary();
            doc.Resources.Add("ResultResources", rd);
            return doc;
        }
    }
}
