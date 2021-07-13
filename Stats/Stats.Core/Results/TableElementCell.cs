using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Results
{
    public class TableElementCell
    {
        internal TableElementCell()
        {
        }

        public object Value
        {
            get;
            set;
        }

        public string FormatString
        {
            get;
            set;
        }

        public string Content
        {
            get
            {
                if (Value == null)
                {
                    return String.Empty;
                }
                else if (FormatString == null)
                {
                    return Value.ToString();
                }
                else
                {
                    return string.Format(FormatString, Value);
                }
            }
        }
    }
}
