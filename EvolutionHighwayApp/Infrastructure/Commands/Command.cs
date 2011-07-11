using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EvolutionHighwayApp.Infrastructure.Commands
{
    public class Command: ICommand
    {
        private readonly Dispatcher _dispatcher;
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _executeAction;
        private bool _canExecuteCache;

        // ReSharper disable InconsistentNaming
        public Command(Action<object> executeAction, Func<object, bool> canExecute, bool executeOnUIThread = true)
        {
            _dispatcher = executeOnUIThread ? Deployment.Current.Dispatcher : null;
            _executeAction = executeAction;
            _canExecute = canExecute;
        }
        // ReSharper restore InconsistentNaming

        #region ICommand Members
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var tempCanExecute = _canExecute(parameter);

            if (_canExecuteCache != tempCanExecute)
            {
                _canExecuteCache = tempCanExecute;

                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, new EventArgs());
            }

            return _canExecuteCache;
        }

        public void Execute(object parameter)
        {
            if (_dispatcher == null || _dispatcher.CheckAccess())
            {
                Debug.WriteLine("Executing command directly");
                _executeAction(parameter);
            }
            else
            {
                Debug.WriteLine("Executing command via dispatcher");
                _dispatcher.BeginInvoke(_executeAction, parameter);
            }
        }
        #endregion
    }
}
