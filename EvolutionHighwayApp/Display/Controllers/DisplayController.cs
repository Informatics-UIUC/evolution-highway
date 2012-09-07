using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Repository.Controllers;
using EvolutionHighwayApp.Settings.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Display.Controllers
{
    public class DisplayController : ModelBase, IDisplayController
    {
        #region ViewModel Bindable Properties

        private bool _showCentromere;
        public bool ShowCentromere
        {
            get { return _showCentromere; }
            private set { NotifyPropertyChanged(() => ShowCentromere, ref _showCentromere, value); }
        }

        private bool _showHeterochromatin;
        public bool ShowHeterochromatin
        {
            get { return _showHeterochromatin; }
            private set { NotifyPropertyChanged(() => ShowHeterochromatin, ref _showHeterochromatin, value); }
        }

        private bool _showConservedSynteny;
        public bool ShowConservedSynteny
        {
            get { return _showConservedSynteny; }
            private set
            {
                if (value) ShowHighlightRegions = true;
                NotifyPropertyChanged(() => ShowConservedSynteny, ref _showConservedSynteny, value);
            }
        }

        private bool _showBreakpointClassification;
        public bool ShowBreakpointClassification
        {
            get { return _showBreakpointClassification; }
            private set
            {
                if (value) ShowHighlightRegions = true;
                NotifyPropertyChanged(() => ShowBreakpointClassification, ref _showBreakpointClassification, value);
            }
        }

        private bool _showHighlightRegions;
        public bool ShowHighlightRegions
        {
            get { return _showHighlightRegions; }
            private set { NotifyPropertyChanged(() => ShowHighlightRegions, ref _showHighlightRegions, value); }
        }

        #endregion

        private IEnumerable<RefGenome> _visibleRefGenomes;
        private Dictionary<RefGenome, IEnumerable<RefChromosome>> _visibleRefChromosomes;
        private Dictionary<RefChromosome, IEnumerable<CompGenome>> _visibleCompGenomes;
        private Dictionary<RefChromosome, IEnumerable<Region>> _highlightRegions;

        private readonly AutoResetEvent _syncLock;

        private readonly IRepositoryController _repositoryController;
        private readonly IEventPublisher _eventPublisher;

        private BreakpointClassificationOptions _breakpointClassificationOptions;

        public DisplayController(IRepositoryController repositoryController, IEventPublisher eventPublisher)
        {
            Debug.WriteLine("{0} instantiated", GetType().Name);

            _syncLock = new AutoResetEvent(true);

            _repositoryController = repositoryController;
            _eventPublisher = eventPublisher;

            InitializeSelections();

            eventPublisher.GetEvent<DataSourceChangedEvent>()
                .Subscribe(e => InitializeSelections());
        }

        private void InitializeSelections()
        {
            _visibleRefGenomes = new RefGenome[0];
            _visibleRefChromosomes = new Dictionary<RefGenome, IEnumerable<RefChromosome>>();
            _visibleCompGenomes = new Dictionary<RefChromosome, IEnumerable<CompGenome>>();
            _highlightRegions = new Dictionary<RefChromosome, IEnumerable<Region>>();

            _breakpointClassificationOptions = null;
        }

        public IEnumerable<RefGenome> GetVisibleRefGenomes()
        {
            return _visibleRefGenomes;
        }

        public IEnumerable<RefChromosome> GetVisibleRefChromosomes()
        {
            return _visibleCompGenomes.Keys;
        }

        public IEnumerable<RefChromosome> GetVisibleRefChromosomes(RefGenome genome)
        {
            return _visibleRefChromosomes.GetValueOrDefault(genome, new RefChromosome[0]);
        }

        public IEnumerable<CompGenome> GetVisibleCompGenomes()
        {
            return _visibleCompGenomes.Values.SelectMany(g => g).ToArray();
        }

        public IEnumerable<CompGenome> GetVisibleCompGenomes(RefChromosome chromosome)
        {
            return _visibleCompGenomes.GetValueOrDefault(chromosome, new CompGenome[0]);
        }

        public IEnumerable<Region> GetHighlightRegions(RefChromosome chromosome)
        {
            return _highlightRegions.GetValueOrDefault(chromosome, new Region[0]);
        }

        public void UpdateDisplay(ICollection<CompGenome> compGenomes)
        {
            _syncLock.WaitOne();

            var visibleCompGenomes = compGenomes.GroupBy(g => g.RefChromosome).ToDictionary(k => k.Key, v => v.AsEnumerable());
            var visibleRefChromosomes = visibleCompGenomes.Keys.GroupBy(c => (RefGenome)c.Genome).ToDictionary(k => k.Key, v => v.AsEnumerable());
            var visibleRefGenomes = visibleRefChromosomes.Keys.ToArray();

            var addedRefGenomes = visibleRefGenomes.Except(_visibleRefGenomes).ToArray();
            var removedRefGenomes = _visibleRefGenomes.Except(visibleRefGenomes).ToArray();

            var addedRefChromosomes = visibleRefChromosomes.GroupJoin(_visibleRefChromosomes, ok => ok.Key, ik => ik.Key,
                (visNow, visOld) => new { Key = visNow.Key, Value = visNow.Value.Except(visOld.SelectMany(c => c.Value))})
                .Where(e => !e.Value.IsEmpty()).ToDictionary(k => k.Key, v => v.Value.ToArray());

            var removedRefChromosomes = _visibleRefChromosomes.GroupJoin(visibleRefChromosomes, ok => ok.Key, ik => ik.Key,
                (visOld, visNow) => new { Key = visOld.Key, Value = visOld.Value.Except(visNow.SelectMany(c => c.Value)) })
                .Where(e => !e.Value.IsEmpty()).ToDictionary(k => k.Key, v => v.Value.ToArray());

            var addedCompGenomes = visibleCompGenomes.GroupJoin(_visibleCompGenomes, ok => ok.Key, ik => ik.Key,
                (visNow, visOld) => new { Key = visNow.Key, Value = visNow.Value.Except(visOld.SelectMany(g => g.Value)) })
                .Where(e => !e.Value.IsEmpty()).ToDictionary(k => k.Key, v => v.Value.ToArray());

            var removedCompGenomes = _visibleCompGenomes.GroupJoin(visibleCompGenomes, ok => ok.Key, ik => ik.Key,
                (visNow, visOld) => new { Key = visNow.Key, Value = visNow.Value.Except(visOld.SelectMany(g => g.Value)) })
                .Where(e => !e.Value.IsEmpty()).ToDictionary(k => k.Key, v => v.Value.ToArray());

            // for all removed reference chromosomes, also remove their associated highlight regions, if any
            removedRefChromosomes.Values.SelectMany(c => c).ForEach(c => _highlightRegions.Remove(c));

            _visibleRefGenomes = visibleRefGenomes;
            _visibleRefChromosomes = visibleRefChromosomes;
            _visibleCompGenomes = visibleCompGenomes;

            var worker = new BackgroundWorker {WorkerReportsProgress = false, WorkerSupportsCancellation = false};

            worker.DoWork += (s, ea) =>
            {
                var waitHandles = new List<ManualResetEvent>();
                var synBlocksLoadedWaitHandle = new ManualResetEvent(false);
                waitHandles.Add(synBlocksLoadedWaitHandle);

                if (ShowCentromere)
                {
                    var centromereLoadedWaitHandle = new ManualResetEvent(false);
                    waitHandles.Add(centromereLoadedWaitHandle);

                    _repositoryController.GetCentromereRegions(_visibleCompGenomes.Keys,
                        cr => centromereLoadedWaitHandle.Set());
                }

                if (ShowHeterochromatin)
                {
                    var heterochromatinLoadedWaitHandle = new ManualResetEvent(false);
                    waitHandles.Add(heterochromatinLoadedWaitHandle);

                    _repositoryController.GetHeterochromatinRegions(_visibleCompGenomes.Keys,
                        hr => heterochromatinLoadedWaitHandle.Set());
                }

                _repositoryController.GetSyntenyBlocks(compGenomes,
                    e => synBlocksLoadedWaitHandle.Set());

                waitHandles.All(w => w.WaitOne());
            };

            worker.RunWorkerCompleted += (s, ea) =>
            {
                Debug.WriteLine("Load completed");

                if (ShowConservedSynteny)
                    SetShowConservedSynteny();

                if (ShowBreakpointClassification && _breakpointClassificationOptions != null)
                    SetShowBreakpointClassification(
                        _breakpointClassificationOptions.Classes, 
                        _breakpointClassificationOptions.MaxThreshold);

                _visibleCompGenomes.ForEach(kvp => _eventPublisher.Publish(new CompGenomeSelectionDisplayEvent(kvp.Key)
                {
                    AddedGenomes = addedCompGenomes.GetValueOrDefault(kvp.Key, new CompGenome[0]),
                    RemovedGenomes = removedCompGenomes.GetValueOrDefault(kvp.Key, new CompGenome[0]),
                    SelectedGenomes = kvp.Value
                }));

                _visibleRefChromosomes.ForEach(kvp => _eventPublisher.Publish(new RefChromosomeSelectionDisplayEvent(kvp.Key)
                {
                    AddedChromosomes = addedRefChromosomes.GetValueOrDefault(kvp.Key, new RefChromosome[0]),
                    RemovedChromosomes = removedRefChromosomes.GetValueOrDefault(kvp.Key, new RefChromosome[0]),
                    SelectedChromosomes = kvp.Value
                }));

                _eventPublisher.Publish(new RefGenomeSelectionDisplayEvent
                {
                    AddedGenomes = addedRefGenomes,
                    RemovedGenomes = removedRefGenomes,
                    SelectedGenomes = _visibleRefGenomes
                });

                _syncLock.Set();
            };

            worker.RunWorkerAsync();
        }

        public void SetShowCentromere(bool visible, Action continuation = null)
        {
            if (!visible)
            {
                ShowCentromere = false;
                if (continuation != null) continuation();
                return;
            }

            _repositoryController.GetCentromereRegions(GetVisibleRefChromosomes().ToArray(),
                centromere =>
                    {
                        centromere.Result.GroupBy(c => c.RefChromosome)
                            .Select(c => new CentromereRegionDisplayEvent(c.Key, c.ToList()))
                            .ForEach(evt => _eventPublisher.Publish(evt));

                        ShowCentromere = visible;

                        if (continuation != null) continuation();
                    });
        }

        public void SetShowHeterochromatin(bool visible, Action continuation = null)
        {
            if (!visible)
            {
                ShowHeterochromatin = false;
                if (continuation != null) continuation();
                return;
            }

            _repositoryController.GetHeterochromatinRegions(GetVisibleRefChromosomes().ToArray(),
                heterochromatin =>
                    {
                        heterochromatin.Result.GroupBy(c => c.RefChromosome)
                            .Select(c => new HeterochromatinRegionDisplayEvent(c.Key, c.ToList()))
                            .ForEach(evt => _eventPublisher.Publish(evt));

                        ShowHeterochromatin = visible;

                        if (continuation != null) continuation();
                    });
        }

        
        public void SetHighlightRegions(RefChromosome chromosome, ICollection<Region> highlightRegions)
        {
            ShowConservedSynteny = false;
            ShowBreakpointClassification = false;

            _highlightRegions[chromosome] = highlightRegions;
            _eventPublisher.Publish(new HighlightRegionDisplayEvent(chromosome, highlightRegions));

            ShowHighlightRegions = true;
        }

        public void SetShowConservedSynteny(Action continuation = null)
        {
            ShowBreakpointClassification = false;
            ShowConservedSynteny = true;

            foreach (var kvp in _visibleCompGenomes)
            {
                var conservedRegions = GetConservedSynteny(kvp.Value).ToList();
                _highlightRegions[kvp.Key] = conservedRegions;
                _eventPublisher.Publish(new HighlightRegionDisplayEvent(kvp.Key, conservedRegions));
            }

            if (continuation != null) continuation();
        }

        public void SetShowBreakpointClassification(IEnumerable<string> classNames, double maxThreshold, Action continuation = null)
        {
            ShowConservedSynteny = false;
            ShowBreakpointClassification = true;

            _breakpointClassificationOptions = new BreakpointClassificationOptions(classNames.ToList(), maxThreshold);

            foreach (var kvp in _visibleCompGenomes)
            {
                var genomes = kvp.Value.ToList();
                var classes = genomes.Where(g => classNames.Contains(g.Name)).ToList();
                var compGenomes = genomes.Except(classes).ToList();
                var classRegions = GetBreakpointClassification(classes, compGenomes, maxThreshold).ToList();

                _highlightRegions[kvp.Key] = classRegions;
                _eventPublisher.Publish(new HighlightRegionDisplayEvent(kvp.Key, classRegions));
            }

            if (continuation != null) continuation();
        }

        public void ClearHighlight()
        {
            _highlightRegions.Clear();
            ShowHighlightRegions = false;
            ShowConservedSynteny = false;
            ShowBreakpointClassification = false;

            _eventPublisher.Publish(new HighlightRegionDisplayEvent(null, null));
        }

        private static IEnumerable<Region> GetConservedSynteny(IEnumerable<CompGenome> compGenomes)
        {
            return GetOverlapRegions(compGenomes.Select(g => g.SyntenyBlocks), ConservedSyntenyHighlightRegionFactory);
        }

        private static IEnumerable<Region> GetBreakpointClassification(IEnumerable<CompGenome> classes, IEnumerable<CompGenome> compGenomes, double maxThreshold)
        {
            var overlapGenomes = GetOverlapRegions(compGenomes.Select(g => g.SyntenyBlocks), BreakpointClassificationHighlightRegionFactory);
            var overlapClassBreakpoints = GetOverlapRegions(classes.Select(c => GetBreakpoints(c, BreakpointClassificationHighlightRegionFactory).MemoizeAll()), BreakpointClassificationHighlightRegionFactory);
            var classRegions = GetOverlapRegions(new[] {overlapGenomes, overlapClassBreakpoints}, BreakpointClassificationHighlightRegionFactory);

            return classRegions.Where(r => r.Span < maxThreshold);
        }

        private static IEnumerable<T> GetOverlapRegions<T>(IEnumerable<IEnumerable<Region>> regionsCollection, Func<double, double, T> regionFactory) where T:Region
        {
            IEnumerable<Tuple<IEnumerable<Region>,T>> emptyRegions = 
                new[] { new Tuple<IEnumerable<Region>, T>(Enumerable.Empty<T>(), null) };

            return
                regionsCollection.Aggregate(emptyRegions,
                    (accRegions, regs) =>
                        from acc in accRegions
                        from region in regs
                        let regions = acc.Item1.Concat(new[] {region})
                        let overlap = GetOverlap(regions, regionFactory)
                        where overlap != null
                        select new Tuple<IEnumerable<Region>, T>(regions, overlap),
                    result => result.Select(r => r.Item2));
        }

        private static T GetOverlap<T>(IEnumerable<Region> regions, Func<double, double, T> regionFactory) where T:Region
        {
            regions = regions.ToArray();
            var x = regions.Max(r => r.Start);
            var y = regions.Min(r => r.End);

            return y >= x ? regionFactory(x, y) : null;
        }

        private static IEnumerable<T> GetBreakpoints<T>(CompGenome compGenome, Func<double, double, T> regionFactory) where T:Region
        {
            var sortedRegions = compGenome.SyntenyBlocks.ToList();
            sortedRegions.Sort((a, b) => a.Start.CompareTo(b.Start));

            double i = 0;
            foreach (var region in sortedRegions)
            {
                if (i < region.Start)
                    yield return regionFactory(i, region.Start);

                i = region.End;
            }

            if (i < compGenome.RefChromosome.Length)
                yield return regionFactory(i, compGenome.RefChromosome.Length);
        }

        private static readonly Func<double, double, ConservedSyntenyHighlightRegion> ConservedSyntenyHighlightRegionFactory = 
            (s, e) => new ConservedSyntenyHighlightRegion(s, e);

        private static readonly Func<double, double, BreakpointClassificationHighlightRegion> BreakpointClassificationHighlightRegionFactory =
           (s, e) => new BreakpointClassificationHighlightRegion(s, e);

    }
}
