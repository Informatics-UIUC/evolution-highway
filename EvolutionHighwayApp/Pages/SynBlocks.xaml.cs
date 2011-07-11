using System;
using System.Linq;
using System.Windows.Navigation;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;

namespace EvolutionHighwayApp.Pages
{
    public partial class SynBlocks
    {
        public SynBlocks()
        {
            InitializeComponent();

            var eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            var dataSourceChangedEvent = eventPublisher.GetEvent<DataSourceChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => _accordion.SelectAll());

            var loadingEvent = eventPublisher.GetEvent<SyntenyBlocksLoadingEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => _loadingIndicator.IsBusy = !e.IsDoneLoading);
            
            Unloaded += delegate
                {
                    dataSourceChangedEvent.Dispose(); 
                    loadingEvent.Dispose();
                    IoC.Container.Release(eventPublisher);
                };
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
