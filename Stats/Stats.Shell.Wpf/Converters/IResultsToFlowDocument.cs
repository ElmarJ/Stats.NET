using System;
using System.Windows.Data;
using System.Windows.Documents;
using Stats.Core.Results;
using Stats.Modules.Presenters;

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
