using EvolutionHighwayApp.Infrastructure.Events;
using EvolutionHighwayApp.Infrastructure.WebServices;

namespace EvolutionHighwayApp.Infrastructure.MVP
{
    public abstract class Presenter<TView, TBindingModel>
        where TView : IView
        where TBindingModel : BindingModel<TBindingModel>, new()
    {
        protected Presenter(TView view, IEventPublisher eventPublisher, IEHDataService dataService)
        {
            View = view;
            EventPublisher = eventPublisher;
            DataService = dataService;
            BindingModel = new TBindingModel();
        }

        protected IEventPublisher EventPublisher { get; private set; }
        protected IEHDataService DataService { get; private set; }
        protected TView View { get; private set; }

        public TBindingModel BindingModel { get; private set; }

        public virtual void Initialize() { }
    }
}
