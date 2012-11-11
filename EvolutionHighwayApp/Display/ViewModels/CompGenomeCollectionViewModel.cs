using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Display.Controllers;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Repository.Controllers;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Display.ViewModels
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
                var key = string.Format("<{0}><{1}>", RefChromosome.Genome.Name, RefChromosome.Name);
                return _repositoryController.CustomTrackMap.GetValueOrDefault(key);
            }
        }

        public AppSettings AppSettings { get; private set; }

        public IDisplayController DisplayController { get { return _displayController; } }

        private RefChromosome _refChromosome;
        public RefChromosome RefChromosome
        {
            get { return _refChromosome; }
            set
            {
                if (_refChromosome == value)
                    return;

                _refChromosome = value;

                CompGenomes.ReplaceWith(_displayController.GetVisibleCompGenomes(_refChromosome));

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

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepositoryController _repositoryController;
        private readonly IDisplayController _displayController;

        private readonly IDisposable _compGenomeSelectionChangedObserver;
        private readonly IDisposable _displaySizeChangedObserver;
        private readonly IDisposable _centromereRegionDisplayEventObserver;
        private readonly IDisposable _customTrackEventObserver;

        public CompGenomeCollectionViewModel()
        {
            CompGenomes = new SmartObservableCollection<CompGenome>();
            AppSettings = IoC.Container.Resolve<AppSettings>();

            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();
            _repositoryController = IoC.Container.Resolve<IRepositoryController>();
            _displayController = IoC.Container.Resolve<IDisplayController>();

            _compGenomeSelectionChangedObserver = _eventPublisher.GetEvent<CompGenomeSelectionDisplayEvent>()
                .Where(e => e.RefChromosome == RefChromosome)
                .ObserveOnDispatcher()
                .Subscribe(OnCompGenomeSelectionDisplay);

            _displaySizeChangedObserver = _eventPublisher.GetEvent<DisplaySizeChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e =>
                    {
                        NotifyPropertyChanged(() => RefChromosome);
                        NotifyPropertyChanged(() => ClipRegion);
                    });

            _centromereRegionDisplayEventObserver = _eventPublisher.GetEvent<ShowCentromereEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => NotifyPropertyChanged(() => ClipRegion));

            _customTrackEventObserver = _eventPublisher.GetEvent<CustomTrackDataLoadedEvent>()
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
            var centromereRegions = _refChromosome.CentromereRegions;

            if (!_displayController.ShowCentromere || centromereRegions.IsEmpty())
            {
                // No centromere to display
                return new RectangleGeometry { RadiusX = 10, RadiusY = 10, Rect = new Rect(0, 0, width, length) };
            }

            var region = centromereRegions.First();
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
            _centromereRegionDisplayEventObserver.Dispose();
            _customTrackEventObserver.Dispose();
            _refChromosome = null;

            CompGenomes.Clear();
        }
    }
}
