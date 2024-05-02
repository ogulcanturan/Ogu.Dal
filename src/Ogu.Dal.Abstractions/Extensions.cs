using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public static string ToProperIncludeProperties(
            this IDictionary<string, string> propertyMappingDictionary, string include, params string[] extras)
        {
            if (propertyMappingDictionary == null)
                throw new ArgumentNullException(nameof(propertyMappingDictionary));

            extras = extras ?? Array.Empty<string>();

            return string.Join(",", TryExtractValuesFromDictionary(propertyMappingDictionary, GetIncludeListOrEmpty(include)).Concat(extras).Distinct());
        }

        public static string ToProperIncludeProperties(this string include, params string[] extras)
        {
            if (include == null)
                throw new ArgumentNullException(nameof(include));

            extras = extras ?? Array.Empty<string>();

            return string.Join(",", GetIncludeListOrEmpty(include).Concat(extras).Distinct());
        }

        public static Expression<Func<T, T>> ToProperColumnsSelectionExpression<T>(
            this IDictionary<string, string> propertyMappingDictionary, string select, params string[] extras)
        {
            if (propertyMappingDictionary == null)
                throw new ArgumentNullException(nameof(propertyMappingDictionary));

            extras = extras ?? Array.Empty<string>();

            var type = typeof(T);

            var parameter = Expression.Parameter(type, "x");

            var distinctSelection =
                TryExtractValuesFromDictionary(propertyMappingDictionary, GetSelectListOrEmpty(select)).Concat(extras)
                    .Distinct().ToArray();

            if (distinctSelection.Length == 0)
                return null;

            var bindings = distinctSelection.Select(name =>
                    {
                        var memberExpression = Expression.PropertyOrField(parameter, name);

                        return Expression.Bind(memberExpression.Member, memberExpression);
                    });

            var body = Expression.MemberInit(Expression.New(type), bindings);

            return Expression.Lambda<Func<T, T>>(body, parameter);
        }

        public static Expression<Func<T, T>> ToColumnsSelectionExpression<T>(this string select, params string[] extras)
        {
            if(select == null) 
                throw new ArgumentNullException(nameof(select));

            extras = extras ?? Array.Empty<string>();

            var type = typeof(T);

            var parameter = Expression.Parameter(type, "x");

            var distinctSelection =
               GetSelectListOrEmpty(select).Concat(extras)
                    .Distinct().ToArray();

            if (distinctSelection.Length == 0)
                return null;

            var bindings = distinctSelection.Select(name =>
            {
                var memberExpression = Expression.PropertyOrField(parameter, name);

                return Expression.Bind(memberExpression.Member, memberExpression);
            });

            var body = Expression.MemberInit(Expression.New(type), bindings);

            return Expression.Lambda<Func<T, T>>(body, parameter);
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

        private static IEnumerable<string> GetSelectListOrEmpty(string select)
        {
            if (string.IsNullOrWhiteSpace(select))
                return Enumerable.Empty<string>();

            return select.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim())
                .Where(i => i != string.Empty);
        }

        private static IEnumerable<string> GetIncludeListOrEmpty(string include)
        {
            if (string.IsNullOrWhiteSpace(include))
                return Enumerable.Empty<string>();

            var substrings = new HashSet<string>();

            var currentSubstring = new StringBuilder();

            foreach (var part in include.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).Where(i => i != string.Empty))
            {
                foreach (var nestedPart in part.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).Where(i => i != string.Empty))
                {
                    if (currentSubstring.Length > 0)
                        currentSubstring.Append('.');

                    currentSubstring.Append(nestedPart);
                    substrings.Add(currentSubstring.ToString());
                }

                currentSubstring.Clear();
            }

            return substrings;
        }

        private static IEnumerable<string> TryExtractValuesFromDictionary(IDictionary<string, string> dictionary, IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                if (dictionary.TryGetValue(key, out var extractedValue))
                {
                    yield return extractedValue;
                }
            }
        }
    }
}