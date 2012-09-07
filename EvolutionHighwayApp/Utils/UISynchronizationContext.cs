using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace EvolutionHighwayApp.Utils
{
    /// <summary>
    /// Singleton class providing the default implementation 
    /// for the <see cref="ISynchronizationContext"/>, specifically for the UI thread.
    /// </summary>
    public class UISynchronizationContext
    {
        DispatcherSynchronizationContext _context;
        Dispatcher _dispatcher;

        #region Singleton implementation

        static readonly UISynchronizationContext instance = new UISynchronizationContext();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>The singleton instance.</value>
        public static UISynchronizationContext Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        public void Initialize()
        {
            EnsureInitialized();
        }

        readonly object _initializationLock = new object();

        void EnsureInitialized()
        {
            if (_dispatcher != null && _context != null)
            {
                return;
            }

            lock (_initializationLock)
            {
                if (_dispatcher != null && _context != null)
                {
                    return;
                }

                try
                {
                    _dispatcher = Deployment.Current.Dispatcher;
                    _context = new DispatcherSynchronizationContext(_dispatcher);
                }
                catch (InvalidOperationException)
                {
                    /* TODO: Make localizable resource. */
                    throw new Exception("Initialised called from non-UI thread.");
                }
            }
        }

        public void Initialize(Dispatcher dispatcher)
        {
            lock (_initializationLock)
            {
                this._dispatcher = dispatcher;
                _context = new DispatcherSynchronizationContext(dispatcher);
            }
        }

        public void InvokeAsynchronously(SendOrPostCallback callback, object state)
        {
            EnsureInitialized();

            _context.Post(callback, state);
        }

        public void InvokeAsynchronously(Action action)
        {
            EnsureInitialized();

            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _dispatcher.BeginInvoke(action);
            }
        }

        public void InvokeSynchronously(SendOrPostCallback callback, object state)
        {
            EnsureInitialized();

            _context.Send(callback, state);
        }

        public void InvokeSynchronously(Action action)
        {
            EnsureInitialized();

            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _context.Send(delegate { action(); }, null);
            }
        }

        public bool InvokeRequired
        {
            get
            {
                EnsureInitialized();
                return !_dispatcher.CheckAccess();
            }
        }
    }
}
