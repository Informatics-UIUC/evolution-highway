using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.State;

namespace EvolutionHighwayApp.Pages
{
    public partial class SynBlocks
    {
        public SynBlocks()
        {
            InitializeComponent();

            var eventPublisher = IoC.Container.Resolve<IEventPublisher>();
            var selections = IoC.Container.Resolve<SelectionsController>();
            var appSettings = IoC.Container.Resolve<AppSettings>();

            var dataSourceChangedEventObserver = eventPublisher.GetEvent<DataSourceChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => _accordion.SelectAll());

            var loadingEventObserver = eventPublisher.GetEvent<SyntenyBlocksLoadingEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => _loadingIndicator.IsBusy = !e.IsDoneLoading);

            var resetZoomEventObserver = eventPublisher.GetEvent<ResetZoomEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e =>
                {
                    if (selections.VisibleCompGenomes.Count > 0)
                        ScaleConverter.DataMaximum = selections.VisibleCompGenomes.Keys.Max(c => c.Length);

                    var desiredSize = appSettings.SynBlocksLayout == Orientation.Vertical ? _scrollViewer.ActualHeight : _scrollViewer.ActualWidth;

                    appSettings.BlockWidth = 24d;
                    appSettings.DisplaySize = desiredSize - 2*appSettings.LabelSize - 80;
                });
            
            Unloaded += delegate
                {
                    dataSourceChangedEventObserver.Dispose(); 
                    loadingEventObserver.Dispose();
                    resetZoomEventObserver.Dispose();
                    IoC.Container.Release(eventPublisher);
                    IoC.Container.Release(selections);
                    IoC.Container.Release(appSettings);
                };
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
