using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace Ogu.Dal.Abstractions
{
    public static class Extensions
    {
//        public static string GenerateHash(params string[] inputs)
//        {
//            var concatenatedInput =
//#if NETSTANDARD2_1
//            string.Join('_', inputs);
//#else
//            string.Join("_", inputs);
//#endif

//#if NET5_0_OR_GREATER
//            var inputBytes = Encoding.UTF8.GetBytes(concatenatedInput);
//            var inputHash = SHA256.HashData(inputBytes);
//            return Convert.ToHexString(inputHash);
//#else
//            using (SHA256 sha256Hash = SHA256.Create())
//            {
//                var hashBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(concatenatedInput));

//                var builder = new StringBuilder();

//                for (var i = 0; i < hashBytes.Length; i++)
//                {
//                    builder.Append(hashBytes[i].ToString("x2"));
//                }

//                return builder.ToString();
//            }
//#endif
//        }

        public static Expression<Func<T, T>> ToColumnsSelectionExpression<T>(this string columns, params char[] separators)
        {
            if (string.IsNullOrWhiteSpace(columns))
                throw new ArgumentException("Property name cannot be null or whitespace.", nameof(columns));

            if (separators == null || separators.Length == 0)
            {
                separators = new[] { ',' };
            }

            var type = typeof(T);

            var parameter = Expression.Parameter(type, "x");

            var bindings = columns.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(name => name.Trim())
                .Where(name => name != string.Empty)
                .Select(name =>
                {
                    var memberExpression = Expression.PropertyOrField(parameter, name);

                    return Expression.Bind(memberExpression.Member, memberExpression);
                });

            var body = Expression.MemberInit(Expression.New(type), bindings);

            var selector = Expression.Lambda<Func<T, T>>(body, parameter);

            return selector;
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null && expr2 != null)
            {
                return expr2;
            }

            if (expr1 != null && expr2 == null)
            {
                return expr1;
            }

            if (expr1 == null)
            {
                throw new ArgumentNullException(nameof(expr1));
            }

            var parameter = Expression.Parameter(typeof(T));

            var visitedExpr1 = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter).Visit(expr1.Body);

            var visitedExpr2 = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter).Visit(expr2.Body);

            var combinedExpr = Expression.AndAlso(visitedExpr1, visitedExpr2);

            return Expression.Lambda<Func<T, bool>>(combinedExpr, parameter);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null && expr2 != null)
            {
                return expr2;
            }

            if (expr1 != null && expr2 == null)
            {
                return expr1;
            }

            if (expr1 == null)
            {
                throw new ArgumentNullException(nameof(expr1));
            }

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