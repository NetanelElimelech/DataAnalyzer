using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer
{
    public class DataBinding
    {
        public bool _ignoreLastIsEnabled { get; set; }

        public bool _twosIsChecked { get; set; }

        public DataBinding()
        {
            SetIsEnabled(false);
            SetIsChecked(false);
        }

        public bool SetIsEnabled(bool isEnabled)
        {
            return _ignoreLastIsEnabled = isEnabled;
        }

        public bool SetIsChecked(bool isChecked)
        {
            return _twosIsChecked = isChecked;
        }
    }
}
