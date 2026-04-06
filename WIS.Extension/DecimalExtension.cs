using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Extension
{
    public static class DecimalExtension
    {
        public static decimal Sum(this decimal value, decimal add)
        {
            if (add > 0 && value > decimal.MaxValue - add)
                return decimal.MaxValue;
            if (add < 0 && value < decimal.MinValue - add)
                return decimal.MinValue;

            return value + add;
        }
    }
}
