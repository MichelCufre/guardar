using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace WIS.Filtering.FilterExpressions
{
    public class FilterExpressionContext
    {
        public List<ParameterExpression> ExpressionParameters { get; set; }

        public FilterExpressionContext()
        {
            this.ExpressionParameters = new List<ParameterExpression>();
        }

        public void RegisterType(Type type, string name)
        {
            this.ExpressionParameters.Add(Expression.Parameter(type, name));
        }

        public ParameterExpression GetType(string name)
        {
            return this.ExpressionParameters.Where(d => d.Name == name).FirstOrDefault();
        }
    }
}
