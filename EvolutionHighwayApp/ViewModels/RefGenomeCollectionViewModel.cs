using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.Views;
using ImageTools;

namespace EvolutionHighwayApp.ViewModels
{
    public class RefGenomeCollectionViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public SmartObservableCollection<RefGenome> RefGenomes { get; private set; }
        public AppSettings AppSettings { get; set; }

        #endregion

        private static RefGenomeCollectionViewer _viewer;
        private readonly IDisposable _dataSourceChangedObserver;
        private readonly IDisposable _refGenomeSelectionChangedObserver;


        public RefGenomeCollectionViewModel(IEventPublisher eventPublisher, RefGenomeCollectionViewer viewer)
            : base(eventPublisher)
        {
            RefGenomes = new SmartObservableCollection<RefGenome>();

            _viewer = viewer;

            _dataSourceChangedObserver = EventPublisher.GetEvent<DataSourceChangedEvent>()
                .Subscribe(e => RefGenomes.Clear());

            _refGenomeSelectionChangedObserver = EventPublisher.GetEvent<RefGenomeSelectionDisplayEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnRefGenomeSelectionDisplay);
        }

        private void OnRefGenomeSelectionDisplay(RefGenomeSelectionDisplayEvent e)
        {
            var selectedGenomes = e.SelectedGenomes.ToList();

            e.RemovedGenomes.ForEach(g => RefGenomes.Remove(g));
            e.AddedGenomes.ForEach(genome => RefGenomes.Insert(selectedGenomes.IndexOf(genome), genome));

            for (var i = 0; i < selectedGenomes.Count; i++)
            {
                var genome = selectedGenomes.ElementAt(i);
                if (RefGenomes.ElementAt(i) == genome) continue;

                RefGenomes.Remove(genome);
                RefGenomes.Insert(i, genome);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _dataSourceChangedObserver.Dispose();
            _refGenomeSelectionChangedObserver.Dispose();
            _viewer = null;

            RefGenomes.Clear();
        }

        public static void CaptureScreen()
        {
            if (_viewer == null) return;

            // create a WriteableBitmap
            var bitmap = new WriteableBitmap((int)_viewer.ActualWidth, (int)_viewer.ActualHeight);

            // render the visual element to the WriteableBitmap
            bitmap.Render(_viewer, null);

            // request an redraw of the bitmap
            bitmap.Invalidate();

            // prompt for a location to save it
            var dialog = new SaveFileDialog { DefaultExt = ".png", Filter = "PNG Files|*.png|JPEG Files|*.jpg|All Files|*.*" };
            if (dialog.ShowDialog() == true)
            {
                // the "using" block ensures the stream is cleaned up when we are finished
                using (var stream = dialog.OpenFile())
                {
                    // encode the stream
                    bitmap.ToImage().WriteToStream(stream, dialog.SafeFileName);
                }
            }
        }
    }
}
