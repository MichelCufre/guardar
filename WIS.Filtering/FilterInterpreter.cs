using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WIS.Exceptions;
using WIS.Filtering.Expressions;
using WIS.Filtering.FilterExpressions;

namespace WIS.Filtering
{
    public class FilterInterpreter : IFilterInterpreter
    {
        private readonly IFilterParser _filterParser;

        public FilterInterpreter(IFilterParser filterParser)
        {
            this._filterParser = filterParser;
        }

        public Expression<Func<T, bool>> Interpret<T>(string filterString)
        {
            return this.InterpretQueryString<T>(filterString);
        }
        public Expression<Func<T, bool>> Interpret<T>(List<FilterCommand> commands)
        {
            var filterString = this.ParseCommandList(commands);

            return this.Interpret<T>(filterString);
        }

        private Expression<Func<T, bool>> InterpretQueryString<T>(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
                return null;

            var context = new FilterExpressionContext();

            context.RegisterType(typeof(T), "type");

            IFilterExpression filter = this._filterParser.Parse(queryString);

            try
            {
                Expression exp = filter.Evaluate(context);

                return Expression.Lambda<Func<T, bool>>(exp, context.ExpressionParameters.ToArray());
            }
            catch (OverflowException ex)
            {
                throw new InvalidFilterException($"General_Sec0_Error_FiltroExcedeLimiteCampo");
            }
            catch (FormatException ex)
            {
                throw new InvalidFilterException($"General_Sec0_Error_FiltroFormatoInvalido");
            }
        }
        private string ParseCommandList(List<FilterCommand> filters)
        {
            List<string> filterString = this.ParseIndividualFilterQuery(filters);

            return string.Join(" AND ", filterString);
        }
        private List<string> ParseIndividualFilterQuery(List<FilterCommand> filters)
        {
            StringBuilder sb = new StringBuilder();

            var filterStringList = new List<string>();

            foreach (var filter in filters)
            {
                bool hasNegation = false;
                var value = filter.Value.ToLower().Trim();
                var prefix = value.Substring(0, 1);

                if (prefix == "!")
                {
                    sb.Append("!(");
                    hasNegation = true;
                    value = value.TrimStart('!');
                    prefix = value.Substring(0, 1);
                }

                sb.Append(":").Append(filter.ColumnId);

                if (prefix == "=" && value.IndexOf('(') > -1 && value.IndexOf(')') > -1)
                {
                    string function = value.Substring(1, value.IndexOf('(') - 1);

                    string valuesWithin = value.Substring(value.IndexOf('(') + 1, value.IndexOf(')') - value.IndexOf('(') - 1);

                    sb.Append("$");
                    sb.Append(function);
                    sb.Append("(");
                    sb.Append(valuesWithin);
                    sb.Append(")");

                    filterStringList.Add(sb.ToString());

                    sb.Clear();
                }
                else
                {
                    if (prefix != "=" && prefix != ">" && prefix != "<")
                    {
                        sb.Append("=");
                        sb.Append("'");
                        sb.Append(value);
                    }
                    else
                    {
                        var compositeExpression = value.Substring(0, 2);

                        if (compositeExpression == ">=" || compositeExpression == "<=")
                        {
                            sb.Append(compositeExpression);
                            sb.Append("'");
                            sb.Append(value.Substring(2));
                        }
                        else
                        {
                            sb.Append(prefix);
                            sb.Append("'");
                            sb.Append(value.Substring(1));
                        }
                    }

                    sb.Append("'");

                    if (hasNegation)
                        sb.Append(")");

                    filterStringList.Add(sb.ToString());

                    sb.Clear();
                }
            }

            return filterStringList;
        }
    }
}
