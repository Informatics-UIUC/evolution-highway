using System;
using System.Windows.Input;

namespace EvolutionHighwayApp.Infrastructure.Commands
{
    public class MenuCommand : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        Func<object, bool> canExecute;
        Action<object> executeAction;
        bool canExecuteCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuCommand"/> class.
        /// </summary>
        /// <param name="executeAction">The execute action.</param>
        /// <param name="canExecute">The can execute.</param>
        public MenuCommand(Action<object> executeAction,
                               Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        #region ICommand Members
        /// <summary>
        /// Defines the method that determines whether the command 
        /// can execute in its current state.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            bool tempCanExecute = canExecute(parameter);

            if (canExecuteCache != tempCanExecute)
            {
                canExecuteCache = tempCanExecute;
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, new EventArgs());
                }
            }

            return canExecuteCache;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        public void Execute(object parameter)
        {
            executeAction(parameter);
        }
        #endregion
    }
}
