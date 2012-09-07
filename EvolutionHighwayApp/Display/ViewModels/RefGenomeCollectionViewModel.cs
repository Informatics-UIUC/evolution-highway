using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using EvolutionHighwayApp.Display.Views;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using ImageTools;

namespace EvolutionHighwayApp.Display.ViewModels
{
    public class RefGenomeCollectionViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public SmartObservableCollection<RefGenome> RefGenomes { get; private set; }
        public AppSettings AppSettings { get; private set; }

        #endregion

        private static RefGenomeCollectionViewer _viewer;

        private readonly IEventPublisher _eventPublisher;
        private readonly IDisposable _dataSourceChangedObserver;
        private readonly IDisposable _refGenomeSelectionChangedObserver;


        public RefGenomeCollectionViewModel(RefGenomeCollectionViewer viewer)
        {
            RefGenomes = new SmartObservableCollection<RefGenome>();
            AppSettings = IoC.Container.Resolve<AppSettings>();

            _viewer = viewer;
            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            _dataSourceChangedObserver = _eventPublisher.GetEvent<DataSourceChangedEvent>()
                .Subscribe(e => RefGenomes.Clear());

            _refGenomeSelectionChangedObserver = _eventPublisher.GetEvent<RefGenomeSelectionDisplayEvent>()
                //.ObserveOnDispatcher()
                .Subscribe(OnRefGenomeSelectionDisplay);
        }

        private void OnRefGenomeSelectionDisplay(RefGenomeSelectionDisplayEvent e)
        {
            Debug.WriteLine("RefGenSelDisplay");
            var selectedGenomes = e.SelectedGenomes.ToList();

            e.RemovedGenomes.Except(e.AddedGenomes).ForEach(g => RefGenomes.Remove(g));
            e.AddedGenomes.Except(RefGenomes).ForEach(genome => RefGenomes.Insert(selectedGenomes.IndexOf(genome), genome));

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
