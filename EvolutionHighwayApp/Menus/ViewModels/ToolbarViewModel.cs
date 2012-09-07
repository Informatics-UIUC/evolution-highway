using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using EvolutionHighwayApp.Display.Controllers;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.Commands;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Selection.Controllers;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Menus.ViewModels
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

        private readonly IEventPublisher _eventPublisher;
        private readonly ISelectionController _selectionController;
        private readonly IDisplayController _displayController;
        private readonly IDisposable _dataSourceChangedObserver;
        private readonly Regex _regexp = new Regex(@"\s*(?<gen>[^#, ]+)\s*#\s*(?<chr>[^#, ]+)\s*#\s*(?<start>\d+)\s*-\s*(?<end>\d+)");

        public ToolbarViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();

            ZoomInCommand = new Command(ZoomIn, canExecute => true);
            ZoomOutCommand = new Command(ZoomOut, canExecute => true);
            ResetZoomCommand = new Command(ResetZoom, canExecute => true);

            GenomeOrientationHorizontalCommand = new Command(GenomeOrientationHorizontal, canExecute => true);
            GenomeOrientationVerticalCommand = new Command(GenomeOrientationVertical, canExecute => true);

            SearchCommand = new Command(Search, canExecute => true);
            SearchEnabled = false;

            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();
            _selectionController = IoC.Container.Resolve<ISelectionController>();
            _displayController = IoC.Container.Resolve<IDisplayController>();

            _dataSourceChangedObserver = _eventPublisher.GetEvent<DataSourceChangedEvent>()
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
            _eventPublisher.Publish(new ResetZoomEvent());
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
                _displayController.GetVisibleRefChromosomes().ForEach(c => _displayController.SetHighlightRegions(c, new Region[0]));
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
                _selectionController.SelectRefGenomesByName(refGenomeNames, 
                    refGenomeSelection =>
                    {
                        var refChrNames = searchQueries.Select(q => q.RefChromosomeName).Distinct().ToList();
                        _selectionController.SelectRefChromosomesByName(refChrNames,
                            refChrSelection =>
                            {
                                var compGenomeNames = refChrSelection.SelectMany(c => c.CompGenomes).Select(g => g.Name).Distinct().ToList();
                                _selectionController.SelectCompGenomesByName(compGenomeNames,
                                    compGenSelection =>
                                    {
                                        _selectionController.ApplySelections();

                                        //TODO: The search query allows searching for a substring of the genome name - just FYI

                                        var chrHighlightRegions =
                                            from chr in refChrSelection
                                            from query in searchQueries
                                            where
                                                chr.Name == query.RefChromosomeName &&
                                                chr.Genome.Name.Contains(query.RefGenomeName)
                                            let chrRegion = new HighlightRegion(query.Start, query.End)
                                            group chrRegion by chr;

                                        chrHighlightRegions.ForEach(cr => _displayController.SetHighlightRegions(cr.Key, cr.ToArray()));
                                    });
                            });
                    });
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _dataSourceChangedObserver.Dispose();
        }
    }
}
