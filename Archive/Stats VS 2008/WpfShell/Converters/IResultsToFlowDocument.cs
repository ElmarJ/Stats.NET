using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Stats.Core;
using Stats.Core.Analysis;
using Stats.Modules.Presenters;
using Stats.Modules.Analysis;
using Stats.Core.Results;
using System.Windows.Documents;

namespace Stats.Shells.Wpf.Converters
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
