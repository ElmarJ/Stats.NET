using System;
using System.Windows.Documents;
using MathLib.Core.Results;
using MathLib.Core.Results.Presenters;
using System.Windows;
using System.Windows.Media;

namespace MathLib.Modules.Presenters
{
    public class FlowDocumentPresenter : Presenter<Block>
    {
        private const int BorderThickness = 1;
        private const int NumberOfHeaderColumns = 1;
        private const int NumberOfHeaderRows = 1;

        public override Block RenderTableElement(TableElement element)
        {
            Table table = new Table();

            //RowGroups:
            TableRowGroup titleRowGroup = new TableRowGroup();
            table.RowGroups.Add(titleRowGroup);
            TableRowGroup headerRowGroup = new TableRowGroup();
            table.RowGroups.Add(headerRowGroup);
            TableRowGroup bodyRowGroup = new TableRowGroup();
            table.RowGroups.Add(bodyRowGroup);

            // Global formatting:
            table.BorderThickness = new Thickness(BorderThickness);
            table.BorderBrush = Brushes.Black;

            titleRowGroup.FontWeight = FontWeights.Bold;
            titleRowGroup.Background = Brushes.AliceBlue;
            headerRowGroup.FontWeight = FontWeights.Bold;

            //Build columns:
            TableColumn headerColumn = new TableColumn();

            table.Columns.Add(headerColumn);
            
            for (int i = 0; i < element.Rows.Count; i++)
            {
                table.Columns.Add(new TableColumn());
            }

            //Title-row:
            TableRow titleRow = new TableRow();
            titleRowGroup.Rows.Add(titleRow);
            TableCell titleCell = new TableCell(new Paragraph(new Run(element.Title)));
            titleCell.ColumnSpan = element.Columns.Count + NumberOfHeaderColumns;
            titleRow.Cells.Add(titleCell);

            //Header-row:           
            TableRow headerRow = new TableRow();
            headerRowGroup.Rows.Add(headerRow);
            headerRow.Cells.Add(new TableCell());
            foreach (TableElementColumn column in element.Columns)
            {
                headerRow.Cells.Add(new TableCell(new Paragraph(new Run(column.Header))));
            }

            //Body-rows:
            foreach (TableElementRow row in element.Rows)
            {
                // Make new tablerow:
                TableRow tableRow = new TableRow();
                
                // Add rowheader:
                TableCell rowHeader = new TableCell(new Paragraph(new Run(row.Header)));
                rowHeader.FontWeight = FontWeights.Bold;
                tableRow.Cells.Add(rowHeader);

                // Add data:
                foreach (TableElementColumn column in element.Columns)
                {
                    Paragraph content = new Paragraph(new Run(row[column.Index].Content));
                    tableRow.Cells.Add(new TableCell(content));
                }

                bodyRowGroup.Rows.Add(tableRow);
            }

            return table;
        }

        public override Block RenderTextElement(MathLib.Core.Results.TextElement element)
        {
            return new Paragraph(new Run(element.Text));
        }

        public override Block RenderWarningElement(WarningElement element)
        {
            return new Paragraph(new Run(element.Text));
        }
    }
}
