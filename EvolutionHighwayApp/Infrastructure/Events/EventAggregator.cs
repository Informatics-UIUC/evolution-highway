using System;
using System.Collections.Generic;
using System.Linq;

namespace EvolutionHighwayApp.Infrastructure.Events
{
    public class EventAggregator : IEventPublisher
    {
        private readonly ISubject<object> _subject = new Subject<object>();

        public IObservable<T> GetEvent<T>()
        {
            return _subject.AsObservable().OfType<T>();
        }

        public void Publish<TEvent>(TEvent @event)
        {
            _subject.OnNext(@event);
        }
    }
}
