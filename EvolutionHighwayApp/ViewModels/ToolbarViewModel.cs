using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.Commands;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.State;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public AppSettings AppSettings { get; set; }

        private bool _searchEnabled;
        public bool SearchEnabled
        {
            get { return _searchEnabled; }
            private set { NotifyPropertyChanged(() => SearchEnabled, ref _searchEnabled, value); }
        }

        public Command ZoomInCommand { get; private set; }
        public Command ZoomOutCommand { get; private set; }
        public Command ResetZoomCommand { get; private set; }

        public Command GenomeOrientationHorizontalCommand { get; private set; }
        public Command GenomeOrientationVerticalCommand { get; private set; }

        public Command SearchCommand { get; private set; }

        #endregion

        private readonly Repository _repository;
        private readonly SelectionsController _selections;
        private readonly IDisposable _dataSourceChangedObserver;
        private readonly Regex _regexp = new Regex(@"\s*(?<gen>[^:, ]+)\s*:\s*(?<chr>[^:, ]+)\s*:\s*(?<start>\d+)\s*-\s*(?<end>\d+)");

        public ToolbarViewModel(IEventPublisher eventPublisher, Repository repository, SelectionsController selections) : base(eventPublisher)
        {
            _repository = repository;
            _selections = selections;

            ZoomInCommand = new Command(ZoomIn, canExecute => true);
            ZoomOutCommand = new Command(ZoomOut, canExecute => true);
            ResetZoomCommand = new Command(ResetZoom, canExecute => true);

            GenomeOrientationHorizontalCommand = new Command(GenomeOrientationHorizontal, canExecute => true);
            GenomeOrientationVerticalCommand = new Command(GenomeOrientationVertical, canExecute => true);

            SearchCommand = new Command(Search, canExecute => true);
            SearchEnabled = false;

            _dataSourceChangedObserver = eventPublisher.GetEvent<DataSourceChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => SearchEnabled = !string.IsNullOrEmpty(e.DataSourceUrl));
        }

        private void ZoomIn(object param)
        {
            Debug.WriteLine("ZoomIn invoked");
            if (AppSettings.DisplaySize + AppSettings.DisplaySizeSmallChange <= AppSettings.DisplaySizeMaximum)
                AppSettings.DisplaySize += AppSettings.DisplaySizeSmallChange;
        }

        private void ZoomOut(object param)
        {
            Debug.WriteLine("ZoomOut invoked");
            if (AppSettings.DisplaySize - AppSettings.DisplaySizeSmallChange >= AppSettings.DisplaySizeMinimum)
                AppSettings.DisplaySize -= AppSettings.DisplaySizeSmallChange;
        }

        private void ResetZoom(object param)
        {
            Debug.WriteLine("ResetZoom invoked");
            EventPublisher.Publish(new ResetZoomEvent());
        }

        private void GenomeOrientationHorizontal(object param)
        {
            AppSettings.SynBlocksLayout = Orientation.Horizontal;
            AppSettings.GenomeLayout = Orientation.Vertical;
            AppSettings.ChromosomeLayout = Orientation.Horizontal;
        }

        private void GenomeOrientationVertical(object param)
        {
            AppSettings.SynBlocksLayout = Orientation.Vertical;
            AppSettings.ChromosomeLayout = Orientation.Horizontal;
            AppSettings.GenomeLayout = Orientation.Horizontal;
        }

        private void Search(object param)
        {
            var searchText = ((TextBox)param).Text;
            Debug.WriteLine("Search invoked for: " + searchText);

            if (searchText.IsEmpty())
            {
                Debug.WriteLine("Clearing search query");
                _selections.SelectedRefChromosomes.ForEach(c => c.HighlightRegions.Clear());
            }
            else
            {
                var searchQueries = new List<SearchQuery>();

                var match = _regexp.Match(searchText);
                while (match.Success)
                {
                    var gen = match.Groups["gen"].Value;
                    var chr = match.Groups["chr"].Value;
                    var start = match.Groups["start"].Value;
                    var end = match.Groups["end"].Value;

                    Debug.WriteLine("Searching for {0}:{1} from {2} to {3}", gen, chr, start, end);

                    searchQueries.Add(new SearchQuery
                    {
                        RefGenomeName = gen,
                        RefChromosomeName = chr,
                        CompGenomeName = null,
                        Start = long.Parse(start),
                        End = long.Parse(end)
                    });

                    match = match.NextMatch();
                }

                var refGenomeNames = searchQueries.Select(q => q.RefGenomeName).Distinct().ToList();
                var refGenomes = refGenomeNames.SelectMany(n => _repository.RefGenomeMap.Keys.Where(g => g.Name.Contains(n))).ToList();

                _selections.SelectedRefGenomes = refGenomes;
                _repository.LoadRefChromosomes(refGenomes, (result, obj) =>
                {
                    //                    var refChromosomes = searchQueries.SelectMany(q => 
                    //                        _repository.RefGenomeMap.Keys.Where(g => 
                    //                            g.Name.Contains(q.RefGenomeName)).SelectMany(g => 
                    //                                _repository.RefGenomeMap[g].Where(c => c.Name == q.RefChromosomeName)));
                    var refChrNames = searchQueries.Select(q => q.RefChromosomeName).Distinct().ToList();
                    var refChromosomes = refChrNames.SelectMany(n =>
                        refGenomes.SelectMany(g => _repository.RefGenomeMap[g].Where(c => c.Name == n))).ToList();

                    refChromosomes.ForEach(c => c.HighlightRegions.Clear());

                    foreach (var query in searchQueries)
                    {
                        var q = query;
                        refChromosomes.Where(c => c.Name == q.RefChromosomeName && c.RefGenome.Name.Contains(q.RefGenomeName))
                            .ForEach(c => c.HighlightRegions.Add(new HighlightRegion(q.Start, q.End)));
                    }

                    _selections.SelectedRefChromosomes = refChromosomes;
                    _repository.LoadCompGenomes(refChromosomes, (result2, obj2) =>
                    {
                        var compGenomes = refChromosomes.SelectMany(c => _repository.RefChromosomeMap[c]).ToList();
                        EventPublisher.Publish(new UpdateSelectionEvent { SelectedCompGenomes = compGenomes });
                        EventPublisher.Publish(new CompGenomeSelectionChangedEvent
                        {
                            AddedGenomes = compGenomes,
                            RemovedGenomes = _selections.SelectedCompGenomes,
                            SelectedGenomes = compGenomes
                        });
                    });
                });


                //                var selectedChromosomes =
                //                    _repository.RefChromosomeMap.Keys.Where(c => selectedChromosomeNames.Contains(c.Name) && _selections.SelectedRefGenomes.Contains(c.RefGenome)).ToList();
                //
                //                EventPublisher.Publish(new RefChromosomeSelectionChangedEvent
                //                {
                //                    AddedChromosomes = selectedChromosomes,
                //                    RemovedChromosomes = _selections.SelectedRefChromosomes,
                //                    SelectedChromosomes = selectedChromosomes
                //                });
                //
                //                var selectedCompGenomes =
                //                    _repository.RefChromosomeMap.Where(kvp => selectedChromosomes.Contains(kvp.Key))
                //                               .SelectMany(kvp => kvp.Value).ToList();
                //
                //                EventPublisher.Publish(new CompGenomeSelectionChangedEvent
                //                {
                //                    AddedGenomes = selectedCompGenomes,
                //                    RemovedGenomes = _selections.SelectedCompGenomes,
                //                    SelectedGenomes = selectedCompGenomes
                //                });
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _dataSourceChangedObserver.Dispose();
        }
    }
}
