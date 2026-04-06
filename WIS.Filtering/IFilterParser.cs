using System;
using System.Collections.Generic;
using System.Text;
using WIS.Filtering.FilterExpressions;

namespace WIS.Filtering
{
    public interface IFilterParser
    {
        IFilterExpression Parse(string value);
    }
}
