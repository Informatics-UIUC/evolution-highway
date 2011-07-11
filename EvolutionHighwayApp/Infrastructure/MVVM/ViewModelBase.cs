using System;
using System.Diagnostics;
using EvolutionHighwayApp.Infrastructure.EventBus;

namespace EvolutionHighwayApp.Infrastructure.MVVM
{
    public abstract class ViewModelBase : ModelBase, IDisposable
    {
        protected IEventPublisher EventPublisher { get; private set; }

        private ViewModelBase()
        {
            Debug.WriteLine("{0} instantiated", GetType().Name);                        
        }
        
        protected ViewModelBase(IEventPublisher eventPublisher) : this()
        {
            EventPublisher = eventPublisher;
        }

        public virtual void Dispose() { }
    }
}
