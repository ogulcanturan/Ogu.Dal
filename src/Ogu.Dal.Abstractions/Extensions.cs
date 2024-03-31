using System;
using System.Linq;
using System.Linq.Expressions;

namespace Ogu.Dal.Abstractions
{
    public static class Extensions
    {
        public static Expression<Func<T, T>> ToColumnsSelectionExpression<T>(this string columns, params char[] separators)
        {
            if (string.IsNullOrWhiteSpace(columns))
                throw new ArgumentException("Property name cannot be null or whitespace.", nameof(columns));

            if (separators == null)
                throw new ArgumentNullException(nameof(separators));

            var type = typeof(T);

            if (separators.Length == 0)
            {
                separators = new[] { ',' };
            }

            var parameter = Expression.Parameter(type, "x");

            var bindings = columns.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(name => Expression.PropertyOrField(parameter, name))
                .Select(member => Expression.Bind(member.Member, member));

            var body = Expression.MemberInit(Expression.New(type), bindings);

            var selector = Expression.Lambda<Func<T, T>>(body, parameter);

            return selector;
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var visitedExpr1 = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter).Visit(expr1.Body);

            var visitedExpr2 = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter).Visit(expr2.Body);

            var combinedExpr = Expression.AndAlso(visitedExpr1, visitedExpr2);

            return Expression.Lambda<Func<T, bool>>(combinedExpr, parameter);
        }

        public static Expression<Func<T, bool>> OrAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var visitedExpr1 = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter).Visit(expr1.Body);

            var visitedExpr2 = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter).Visit(expr2.Body);

            var combinedExpr = Expression.OrElse(visitedExpr1, visitedExpr2);

            return Expression.Lambda<Func<T, bool>>(combinedExpr, parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                return node == _oldValue ? _newValue : base.Visit(node);
            }
        }
    }
}