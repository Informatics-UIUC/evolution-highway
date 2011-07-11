using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Expression = System.Linq.Expressions.Expression;

namespace EvolutionHighwayApp.Infrastructure.Extensions
{
    public static class Extensions
    {
        public static void Raise<T>(this PropertyChangedEventHandler notifyPropertyChangedEventHandler,
                                    Expression<Func<object>> expr,
                                    ref T oldValue, T newValue,
                                    bool alwaysNotify = false)
        {
            if (!alwaysNotify && !Equals(oldValue, default(T)) && oldValue.Equals(newValue)) 
                return;

            oldValue = newValue;

            Raise(notifyPropertyChangedEventHandler, expr);
        }

        public static void Raise(this PropertyChangedEventHandler notifyPropertyChangedEventHandler, 
                                 Expression<Func<object>> expr)
        {
            if (notifyPropertyChangedEventHandler == null) return;

            var memberExpression = expr.Body is UnaryExpression ?
                ((UnaryExpression)expr.Body).Operand as MemberExpression : expr.Body as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException("'expr' should be a member expression", "expr");

            var vmExpression = memberExpression.Expression as ConstantExpression;
            var vmlambda = Expression.Lambda(vmExpression);
            var vmFunc = vmlambda.Compile();
            var vm = vmFunc.DynamicInvoke();

            var propertyName = memberExpression.Member.Name;
            notifyPropertyChangedEventHandler(vm, new PropertyChangedEventArgs(propertyName));
        }
    }
}
