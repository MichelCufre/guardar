using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace WIS.Filtering
{
    public interface IFilterInterpreter
    {
        Expression<Func<T, bool>> Interpret<T>(string filterString);
        Expression<Func<T, bool>> Interpret<T>(List<FilterCommand> commands);
    }
}
