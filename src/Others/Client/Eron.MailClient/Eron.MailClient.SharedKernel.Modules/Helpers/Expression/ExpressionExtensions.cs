using System;
using System.Linq.Expressions;

namespace Eron.MailClient.SharedKernel.Modules.Helpers.Expression
{
    public static class ExpressionExtensions
    {


        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(
                System.Linq.Expressions.Expression.AndAlso(left, right), parameter);
        }



        private class ReplaceExpressionVisitor
            : ExpressionVisitor
        {
            private readonly System.Linq.Expressions.Expression _oldValue;
            private readonly System.Linq.Expressions.Expression _newValue;

            public ReplaceExpressionVisitor(System.Linq.Expressions.Expression oldValue,
                System.Linq.Expressions.Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
}
