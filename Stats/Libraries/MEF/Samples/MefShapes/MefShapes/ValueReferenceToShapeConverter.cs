//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Samples.MefShapes.Shapes;
using Microsoft.Samples.MefShapes.Shapes.Library;

namespace Microsoft.Samples.MefShapes
{
    public class ValueReferenceToShapeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                Export<IShape, IShapeMetadata> valueReference = value as Export<IShape, IShapeMetadata>;

                if (valueReference != null)
                {
                    return valueReference.GetExportedObject();
                }

                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "value is {0}", value.GetType().FullName));
            }

            throw new ArgumentNullException("value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
