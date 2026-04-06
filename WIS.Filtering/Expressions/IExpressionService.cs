using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WIS.Filtering.Expressions
{
    public interface IExpressionService
    {
        Expression StringContains(Expression paramExpLeft, Expression paramExpRight);
        Expression StringStartsWith(Expression paramExpLeft, Expression paramExpRight);
        Expression StringEndsWith(Expression paramExpLeft, Expression paramExpRight);

        Expression DateTimeContains(Expression paramExpLeft, Expression paramExpRight);
        Expression DateTimeStartsWith(Expression paramExpLeft, Expression paramExpRight);
        Expression DateTimeEndsWith(Expression paramExpLeft, Expression paramExpRight);

        Expression NumberContains(Expression paramExpLeft, Expression paramExpRight);
        Expression NumberStartsWith(Expression paramExpLeft, Expression paramExpRight);
        Expression NumberEndsWith(Expression paramExpLeft, Expression paramExpRight);

        Expression Equals(Expression paramExpLeft, Expression paramExpRight);
        Expression GreaterThan(Expression paramExpLeft, Expression paramExpRight);
        Expression GreaterThanOrEqual(Expression paramExpLeft, Expression paramExpRight);
        Expression LessThan(Expression paramExpLeft, Expression paramExpRight);
        Expression LessThanOrEqual(Expression paramExpLeft, Expression paramExpRight);

        Expression In(Expression paramExpLeft, Expression paramExpRight);
    }
}
