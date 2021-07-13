using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using MathLib.Core;
using MathLib.Core.Analysis;
using MathLib.Modules.Presenters;
using MathLib.Modules.Analysis;
using MathLib.Core.Results;
using System.Windows.Documents;

namespace WpfShell.Converters
{
    class ResultsToFlowDocument: IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                IResults results = (IResults)value;
                FlowDocument document = new FlowDocument();
                FlowDocumentPresenter presenter = new FlowDocumentPresenter();

                foreach (IElement element in results.Elements)
                {
                    document.Blocks.Add(element.Render<Block>(presenter));
                }

                return document;

            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new InvalidCastException();
        }

        #endregion
    }
}
