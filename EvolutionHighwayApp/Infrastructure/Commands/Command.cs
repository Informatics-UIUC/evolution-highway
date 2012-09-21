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
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _executeAction;

        // ReSharper disable InconsistentNaming
        public Command(Action<object> executeAction, Predicate<object> canExecute = null, bool executeOnUIThread = true)
        {
            if (executeAction == null)
                throw new ArgumentNullException("executeAction");

            _dispatcher = executeOnUIThread ? Deployment.Current.Dispatcher : null;
            _executeAction = executeAction;
            _canExecute = canExecute;
        }
        // ReSharper restore InconsistentNaming

        #region ICommand Members
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void UpdateCanExecute()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
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
