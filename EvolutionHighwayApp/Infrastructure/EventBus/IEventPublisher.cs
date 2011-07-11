using System;

namespace EvolutionHighwayApp.Infrastructure.EventBus
{
    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent @event);
        IObservable<TEvent> GetEvent<TEvent>();
    }
}
