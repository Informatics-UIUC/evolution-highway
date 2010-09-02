﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Navigation;
using EvolutionHighwayModel;

namespace EvolutionHighwayApp.Views
{
    public partial class SynBlocks : Page
    {
        private readonly IEHDataService _serviceProxy = 
            EHDataService.CreateServiceProxy(new EndpointAddress("http://leovip027.ncsa.uiuc.edu:8080"));

        private List<Genome> _genomes;
        private ILookup<string, Genome> _genomeNameLookup;

        private static readonly ChromosomeNameComparer ChromosomeNameComparer = new ChromosomeNameComparer();


        public SynBlocks()
        {
            InitializeComponent();

            biGenomes.IsBusy = true;

            _serviceProxy.BeginListGenomes(
                asyncResult =>
                    {
                        _genomes = _serviceProxy.EndListGenomes(asyncResult);
                        _genomes.Sort((a, b) => a.Name.CompareTo(b.Name));
                        _genomeNameLookup = _genomes.ToLookup(g => g.Name, g => g);

                        Dispatcher.BeginInvoke(() =>
                            {
                                lstGenomes.ItemsSource = from genome in _genomes
                                                            select genome.Name;
                                biGenomes.IsBusy = false;
                                accordion.SelectAll();
                            }
                        );
                    }
            );
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void OnGenomesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bw = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };

            bw.DoWork += (s, ea) =>
                {
                    var waitHandles = from genomeName in e.AddedItems.Cast<string>()
                                      let genome = _genomeNameLookup[genomeName].First()
                                      where genome.Chromosomes == null
                                      let mre = new ManualResetEvent(false)
                                      let completed = _serviceProxy.BeginListChromosomes(genome.Name,
                                                  asyncResult =>
                                                  {
                                                      var chromosomes = _serviceProxy.EndListChromosomes(asyncResult);
                                                      chromosomes.Sort((a, b) => ChromosomeNameComparer.Compare(a.Name, b.Name));
                                                      genome.Chromosomes = chromosomes;
                                                      ((ManualResetEvent) asyncResult.AsyncState).Set();
                                                  }, mre).IsCompleted
                                      where !completed
                                      select mre;

                    waitHandles.All(w => w.WaitOne());
                };

            bw.RunWorkerCompleted += (s, ea) =>
                {
                    var selectedChromosomes = (from chrName in lstChromosomes.SelectedItems.Cast<string>() select chrName).ToList();
                    lstChromosomes.SelectionChanged -= OnChromosomesSelectionChanged;
                    lstChromosomes.ItemsSource = (from genomeName in lstGenomes.SelectedItems.Cast<string>()
                                                  let genome = _genomeNameLookup[genomeName].First()
                                                  from chromosome in genome.Chromosomes
                                                  select chromosome.Name).Distinct().OrderBy(a => a, ChromosomeNameComparer);
                    selectedChromosomes.ForEach(chrName => lstChromosomes.SelectedItems.Add(chrName));

                    biChromosomes.IsBusy = false;
                    lstGenomes.IsEnabled = true;
                    lstSpecies.IsEnabled = true;

                    OnChromosomesSelectionChanged(lstChromosomes, 
                        new SelectionChangedEventArgs(new List<string>(), lstChromosomes.SelectedItems.Cast<string>().ToList()));
                    lstChromosomes.SelectionChanged += OnChromosomesSelectionChanged;
                };

            biChromosomes.IsBusy = true;
            lstGenomes.IsEnabled = false;
            lstSpecies.IsEnabled = false;

            bw.RunWorkerAsync();
        }

        private void OnChromosomesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bw = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };

            bw.DoWork += (s, ea) =>
            {
                var waitHandles = from genomeName in lstGenomes.SelectedItems.Cast<string>()
                                  let genome = _genomeNameLookup[genomeName].First()
                                  from chromosome in genome.Chromosomes
                                  where e.AddedItems.Contains(chromosome.Name) && chromosome.ComparativeSpecies == null
                                  let mre = new ManualResetEvent(false)
                                  let completed = _serviceProxy.BeginListSpecies(genome.Name, chromosome.Name,
                                              asyncResult =>
                                              {
                                                  var species = _serviceProxy.EndListSpecies(asyncResult);
                                                  species.Sort((a, b) => a.SpeciesName.CompareTo(b.SpeciesName));
                                                  chromosome.ComparativeSpecies = species;
                                                  ((ManualResetEvent) asyncResult.AsyncState).Set();
                                              }, mre).IsCompleted
                                  where !completed 
                                  select mre;

                waitHandles.All(w => w.WaitOne());
            };

            bw.RunWorkerCompleted += (s, ea) =>
            {
                var selectedSpecies = (from speciesName in lstSpecies.SelectedItems.Cast<string>() select speciesName).ToList();
                lstSpecies.SelectionChanged -= OnSpeciesSelectionChanged;
                lstSpecies.ItemsSource = (from genomeName in lstGenomes.SelectedItems.Cast<string>()
                                              let genome = _genomeNameLookup[genomeName].First()
                                              from chromosome in genome.Chromosomes
                                              where lstChromosomes.SelectedItems.Contains(chromosome.Name)
                                              from species in chromosome.ComparativeSpecies
                                              orderby species.SpeciesName
                                              select species.SpeciesName).Distinct();
                selectedSpecies.ForEach(speciesName => lstSpecies.SelectedItems.Add(speciesName));

                biSpecies.IsBusy = false;
                lstGenomes.IsEnabled = true;
                lstChromosomes.IsEnabled = true;

                OnSpeciesSelectionChanged(lstSpecies, 
                    new SelectionChangedEventArgs(new List<string>(), lstSpecies.SelectedItems.Cast<string>().ToList()));
                lstSpecies.SelectionChanged += OnSpeciesSelectionChanged;
            };

            biSpecies.IsBusy = true;
            lstGenomes.IsEnabled = false;
            lstChromosomes.IsEnabled = false;

            bw.RunWorkerAsync();
        }

        private void OnSpeciesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bw = new BackgroundWorker { WorkerReportsProgress = false, WorkerSupportsCancellation = false };

            bw.DoWork += (s, ea) =>
            {
                var waitHandles = from genomeName in lstGenomes.SelectedItems.Cast<string>()
                                  let genome = _genomeNameLookup[genomeName].First()
                                  from chromosome in genome.Chromosomes
                                  where lstChromosomes.SelectedItems.Contains(chromosome.Name)
                                  from species in chromosome.ComparativeSpecies
                                  where e.AddedItems.Contains(species.SpeciesName) && species.AncestorRegions == null
                                  let mre = new ManualResetEvent(false)
                                  let completed = _serviceProxy.BeginListSynblocks(genome.Name, chromosome.Name, species.SpeciesName,
                                               asyncResult =>
                                               {
                                                   species.AncestorRegions = _serviceProxy.EndListSynblocks(asyncResult);
                                                   ((ManualResetEvent) asyncResult.AsyncState).Set();
                                               }, mre).IsCompleted
                                  where !completed
                                  select mre;

                waitHandles.All(w => w.WaitOne());
            };

            bw.RunWorkerCompleted += (s, ea) =>
            {
                if (lstSpecies.SelectedItems.Count > 0)
                    genomesViewer.DataContext =
                        from genome in _genomes
                        where lstGenomes.SelectedItems.Contains(genome.Name)
                        select new Genome
                                {
                                    Name = genome.Name,
                                    Chromosomes =
                                        from chromosome in genome.Chromosomes
                                        where lstChromosomes.SelectedItems.Contains(chromosome.Name)
                                        select new Chromosome
                                                {
                                                    Name = chromosome.Name,
                                                    ComparativeSpecies =
                                                        from species in chromosome.ComparativeSpecies
                                                        where lstSpecies.SelectedItems.Contains(species.SpeciesName)
                                                        select species
                                                }
                                };
                else
                    genomesViewer.DataContext = null;

                biViewer.IsBusy = false;
                lstGenomes.IsEnabled = true;
                lstChromosomes.IsEnabled = true;
                lstSpecies.IsEnabled = true;
            };

            biViewer.IsBusy = true;
            lstGenomes.IsEnabled = false;
            lstChromosomes.IsEnabled = false;
            lstSpecies.IsEnabled = false;

            bw.RunWorkerAsync();
        }
    }

    internal class ChromosomeNameComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int nx, ny;

            if (int.TryParse(x, out nx) && int.TryParse(y, out ny))
                return nx.CompareTo(ny);

            return x.CompareTo(y);
        }
    }
}
