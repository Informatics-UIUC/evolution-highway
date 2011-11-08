using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.State;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.ViewModels
{
    public class CompGenomeCollectionViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public SmartObservableCollection<CompGenome> CompGenomes { get; private set; }

        public SmartObservableCollection<CustomTrackRegion> CustomTracks
        {
            get
            {
                if (RefChromosome == null) return null;
                var key = string.Format("<{0}><{1}>", RefChromosome.RefGenome.Name, RefChromosome.Name);
                return _repository.CustomTrackMap.GetValueOrDefault(key);
            }
        }

        public AppSettings AppSettings { get; set; }

        private RefChromosome _refChromosome;
        public RefChromosome RefChromosome
        {
            get { return _refChromosome; }
            set
            {
                if (RefChromosome == value) return;
                _refChromosome = value;

                if (_selections.VisibleCompGenomes.ContainsKey(_refChromosome))
                    CompGenomes.AddRange(_selections.VisibleCompGenomes[_refChromosome]);

                NotifyPropertyChanged(() => RefChromosome);
                NotifyPropertyChanged(() => ClipRegion);
                NotifyPropertyChanged(() => CustomTracks);
            }
        }

        private int _blockWidth;
        public int BlockWidth
        {
            get { return _blockWidth; }
            set
            {
                NotifyPropertyChanged(() => BlockWidth, ref _blockWidth, value);
                NotifyPropertyChanged(() => ClipRegion);
            }
        }

        public Geometry ClipRegion
        {
            get { return GetClipRegion(); }
        }

        #endregion

        private static readonly ScaleConverter ScaleConverter = new ScaleConverter();

        private readonly Repository _repository;
        private readonly SelectionsController _selections;
        private readonly IDisposable _compGenomeSelectionChangedObserver;
        private readonly IDisposable _displaySizeChangedObserver;
        private readonly IDisposable _showCentromereObserver;
        private readonly IDisposable _customTrackEventObserver;

        public CompGenomeCollectionViewModel(SelectionsController selections, IEventPublisher eventPublisher, Repository repository)
            : base(eventPublisher)
        {
            _repository = repository;
            _selections = selections;

            CompGenomes = new SmartObservableCollection<CompGenome>();

            _compGenomeSelectionChangedObserver = EventPublisher.GetEvent<CompGenomeSelectionDisplayEvent>()
                .Where(e => e.RefChromosome == RefChromosome)
                .ObserveOnDispatcher()
                .Subscribe(OnCompGenomeSelectionDisplay);

            _displaySizeChangedObserver = EventPublisher.GetEvent<DisplaySizeChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e =>
                    {
                        NotifyPropertyChanged(() => RefChromosome);
                        NotifyPropertyChanged(() => ClipRegion);
                    });

            _showCentromereObserver = EventPublisher.GetEvent<ShowCentromereEvent>()
                .Subscribe(e => NotifyPropertyChanged(() => ClipRegion));

            _customTrackEventObserver = EventPublisher.GetEvent<CustomTrackDataLoadedEvent>()
                .Subscribe(e => NotifyPropertyChanged(() => CustomTracks));
        }

        private void OnCompGenomeSelectionDisplay(CompGenomeSelectionDisplayEvent e)
        {
            var selectedGenomes = e.SelectedGenomes.ToList();

            e.RemovedGenomes.Except(e.AddedGenomes).ForEach(g => CompGenomes.Remove(g));
            e.AddedGenomes.Except(CompGenomes).ForEach(genome => CompGenomes.Insert(selectedGenomes.IndexOf(genome), genome));

            for (var i = 0; i < selectedGenomes.Count; i++)
            {
                var genome = selectedGenomes.ElementAt(i);
                if (CompGenomes.ElementAt(i) == genome) continue;

                CompGenomes.Remove(genome);
                CompGenomes.Insert(i, genome);
            }

            if (!e.AddedGenomes.IsEmpty() || !e.RemovedGenomes.IsEmpty())
                NotifyPropertyChanged(() => ClipRegion);
        }

        private Geometry GetClipRegion()
        {
            if (CompGenomes.Count == 0 || BlockWidth == 0) return null;

            var length = (double) ScaleConverter.Convert(RefChromosome.Length, null, null, null);
            var width = BlockWidth * CompGenomes.Count;

            if (RefChromosome.CentromereRegions.Count == 0 || !AppSettings.ShowCentromere)
            {
                // No centromere to display
                return new RectangleGeometry { RadiusX = 10, RadiusY = 10, Rect = new Rect(0, 0, width, length) };
            }

            var region = RefChromosome.CentromereRegions[0];
            var scaledStart = (double) ScaleConverter.Convert(region.Start, null, null, null);
            var scaledSpan = (double) ScaleConverter.Convert(region.Span, null, null, null);

            var pg = new PathGeometry();
            var pf = new PathFigure { IsClosed = true, IsFilled = true, StartPoint = new Point(10, 0) };
            pf.Segments.Add(new LineSegment { Point = new Point(width - 10, 0) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(width, 10), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new LineSegment { Point = new Point(width, scaledStart + scaledSpan / 2d - 10) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(width - 5, scaledStart + scaledSpan / 2d), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(width, scaledStart + scaledSpan / 2d + 10), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new LineSegment { Point = new Point(width, length - 10) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(width - 10, length), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new LineSegment { Point = new Point(10, length) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(0, length - 10), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new LineSegment { Point = new Point(0, scaledStart + scaledSpan / 2d + 10) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(5, scaledStart + scaledSpan / 2d), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(0, scaledStart + scaledSpan / 2d - 10), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new LineSegment { Point = new Point(0, 10) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(10, 0), SweepDirection = SweepDirection.Clockwise });
            pg.Figures.Add(pf);

            return pg;
        }

        public override void Dispose()
        {
            base.Dispose();

            _compGenomeSelectionChangedObserver.Dispose();
            _displaySizeChangedObserver.Dispose();
            _showCentromereObserver.Dispose();
            _customTrackEventObserver.Dispose();
            _refChromosome = null;

            CompGenomes.Clear();
        }
    }
}
