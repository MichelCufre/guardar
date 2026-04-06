using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Common.Extensions
{
    public static class BoolExtension
    {
        public static string BooleanToString(this bool value)
        {
            return value ? "S" : "N";
        }

        public static string IsChecked(this bool value)
        {
            return value ? "true" : "false";
        }
    }
}
