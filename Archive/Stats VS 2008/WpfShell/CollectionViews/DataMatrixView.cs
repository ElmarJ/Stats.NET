using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using Stats.Core.Data;
using System.Collections.ObjectModel;

namespace Microsoft.Windows.Controls.CollectionViews
{
    class DataMatrixView: CollectionView, IItemProperties
    {
        DataMatrix matrix;
        DataMatrixView(DataMatrix matrix) :
            base(matrix.Records)
        {
            this.matrix = matrix;
        }

        #region IItemProperties Members

        public ReadOnlyCollection<ItemPropertyInfo> ItemProperties
        {
            get
            {
                var properties = new List<ItemPropertyInfo>();
                
                foreach (IVariable variable in matrix.Variables)
                {
                    properties.Add(new ItemPropertyInfo(variable.Name, typeof(object), new object()));
                }
                
                return new ReadOnlyCollection<ItemPropertyInfo>(properties);
            }
        }

        #endregion
    }
}
