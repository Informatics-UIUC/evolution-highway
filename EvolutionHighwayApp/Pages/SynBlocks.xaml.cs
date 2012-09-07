using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Display.Controllers;
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
            var displayController = IoC.Container.Resolve<IDisplayController>();
            var appSettings = IoC.Container.Resolve<AppSettings>();

            var loadingEventObserver = eventPublisher.GetEvent<SyntenyBlocksLoadingEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => _loadingIndicator.IsBusy = !e.IsDoneLoading);

            var resetZoomEventObserver = eventPublisher.GetEvent<ResetZoomEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e =>
                {
                    var visibleRefChromosomes = displayController.GetVisibleRefChromosomes();
                    if (!visibleRefChromosomes.IsEmpty())
                        ScaleConverter.DataMaximum = visibleRefChromosomes.Max(c => c.Length);

                    var desiredSize = appSettings.SynBlocksLayout == Orientation.Vertical ? _scrollViewer.ActualHeight : _scrollViewer.ActualWidth;

                    appSettings.BlockWidth = 24d;
                    appSettings.DisplaySize = desiredSize - 2*appSettings.LabelSize - 80;
                });
            
            Unloaded += delegate
                {
                    loadingEventObserver.Dispose();
                    resetZoomEventObserver.Dispose();
                    IoC.Container.Release(eventPublisher);
                    IoC.Container.Release(displayController);
                    IoC.Container.Release(appSettings);
                };
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
