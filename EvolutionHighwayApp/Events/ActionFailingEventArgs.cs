using System;

namespace EvolutionHighwayApp.Events
{
    public class ActionFailingEventArgs<T>
    {
        public Exception Error { get; private set; }
        public T Context { get; private set; }
        public bool Retry { get; set; }

        public ActionFailingEventArgs(Exception error, T context = default(T))
        {
            Error = error;
            Context = context;
            Retry = false;
        }
    }
}
