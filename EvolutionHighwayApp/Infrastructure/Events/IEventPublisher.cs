using System;

namespace EvolutionHighwayApp.Infrastructure.Events
{
    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent @event);
        IObservable<TEvent> GetEvent<TEvent>();
    }
}
