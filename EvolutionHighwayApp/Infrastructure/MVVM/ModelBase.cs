using System;
using System.ComponentModel;
using System.Linq.Expressions;
using EvolutionHighwayApp.Infrastructure.Extensions;

namespace EvolutionHighwayApp.Infrastructure.MVVM
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(Expression<Func<object>> expr)
        {
            PropertyChanged.Raise(expr);
        }

        protected void NotifyPropertyChanged<T>(Expression<Func<object>> expr, ref T oldValue, T newValue, bool alwaysNotify = false)
        {
            PropertyChanged.Raise(expr, ref oldValue, newValue, alwaysNotify);
        }
    }
}
