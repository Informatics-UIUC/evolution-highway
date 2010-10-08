using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using System.Xml.XPath;
using EvolutionHighwayModel;
using EvolutionHighwayWidgets.Converters;

namespace EvolutionHighwayApp.Views
{
    public partial class SynBlocks : Page
    {
        private IEHDataService _serviceProxy;

        private List<Genome> _genomes;
        private ILookup<string, Genome> _genomeNameLookup;

        private static readonly ChromosomeNameComparer ChromosomeNameComparer = new ChromosomeNameComparer();

//        private List<Genome> _genomes = GetFakeGenomes();


        public SynBlocks()
        {
            InitializeComponent();
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            var dataSourcesSrc = Application.Current.Resources["dataSourceConfigUrl"].ToString();
            var webClient = new WebClient();
            webClient.OpenReadCompleted += delegate(object o, OpenReadCompletedEventArgs ea)
                                           {
                                               if (ea.Error != null)
                                               {
                                                   MessageBox.Show(string.Format("Cannot load {0}:\n{1}", dataSourcesSrc, ea.Error.Message));
                                                   return;
                                               }

                                               using (var s = ea.Result)
                                               {
                                                   var docDataSources = XDocument.Load(s);
                                                   var xmlDataSources = docDataSources.XPathSelectElements("//datasource");
                                                   cbDataSources.ItemsSource = from ds in xmlDataSources 
                                                                               where ds.Attribute("name") != null && ds.Attribute("serviceAddress") != null
                                                                               select new Tuple<string, string>(ds.Attribute("name").Value, ds.Attribute("serviceAddress").Value);
                                               }
                                           };
            webClient.OpenReadAsync(new Uri(dataSourcesSrc, UriKind.RelativeOrAbsolute));
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void OnDataSourceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _serviceProxy = EHDataService.CreateServiceProxy(new EndpointAddress(cbDataSources.SelectedValue.ToString()));

            biGenomes.IsBusy = true;
            accordion.SelectAll();

            _genomes = null;
            lstGenomes.ItemsSource = null;
            lstChromosomes.ItemsSource = null;
            lstSpecies.ItemsSource = null;

            if (_genomes == null || _genomes.Count == 0)
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
                                                       }
                                );
                        }
                    );

            //            _genomes.Sort((a, b) => a.Name.CompareTo(b.Name));
            //            _genomeNameLookup = _genomes.ToLookup(g => g.Name, g => g);
            //            lstGenomes.ItemsSource = from genome in _genomes
            //                                     select genome.Name;
            //            biGenomes.IsBusy = false;
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
                                                      //chromosomes.ForEach(chr => chr.Genome = genome);
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
                                                  //species.ForEach(sp => sp.Chromosome = chromosome);
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
                                                   var ancestorRegions = _serviceProxy.EndListSynblocks(asyncResult);
                                                   //ancestorRegions.ForEach(ar => ar.ComparativeSpecies = species);
                                                   species.AncestorRegions = ancestorRegions;
                                                   ((ManualResetEvent) asyncResult.AsyncState).Set();
                                               }, mre).IsCompleted
                                  where !completed
                                  select mre;

                waitHandles.All(w => w.WaitOne());
            };

            bw.RunWorkerCompleted += (s, ea) =>
            {
                if (lstSpecies.SelectedItems.Count > 0)
                {
                    var data =
                        (from genome in _genomes
                        where lstGenomes.SelectedItems.Contains(genome.Name)
                        select new Genome
                                {
                                    Name = genome.Name,
                                    Chromosomes =
                                        (from chromosome in genome.Chromosomes
                                        where lstChromosomes.SelectedItems.Contains(chromosome.Name)
                                        select new Chromosome
                                                {
                                                    Name = chromosome.Name,
                                                    ComparativeSpecies =
                                                        (from species in chromosome.ComparativeSpecies
                                                        where lstSpecies.SelectedItems.Contains(species.SpeciesName)
                                                        select species).ToList()
                                                }).ToList()
                                }).ToList();

                    var bpMax = 0d;

                    data.ForEach(gen =>
                                 gen.Chromosomes.ForEach(chr =>
                                    {
                                        chr.Genome = gen;
                                        chr.ComparativeSpecies.ForEach(spc =>
                                            {
                                                spc.Chromosome = chr;
                                                spc.AncestorRegions.ForEach(ar =>
                                                    {
                                                        ar.ComparativeSpecies = spc;
                                                        bpMax = Math.Max(bpMax, ar.End);
                                                    });
                                            });
                                    }));

                    ScaleConverter.Scale = bpMax/1000;
                    txtDataScale.Text = ScaleConverter.Scale.ToString();

                    genomesViewer.DataContext = data;
                }
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


        public static List<Genome> GetFakeGenomes()
        {
            var genomes = new List<Genome>();

            for (int i = 0; i <= 4; i++)
            {
                Genome g = new Genome();
                g.Name = "G" + i;
                g.Chromosomes = new List<Chromosome>();

                for (int j = 0; j <= 5; j++)
                {
                    Chromosome c = new Chromosome();
                    c.Name = string.Format("Chr{0}", j);
                    //c.Genome = g;
                    c.ComparativeSpecies = new List<ComparativeSpecies>();

                    int x = 0;
                    foreach (var species in new string[] { "dog", "mouse", "cat" })
                    {
                        ComparativeSpecies cs = new ComparativeSpecies();
                        cs.SpeciesName = string.Format("{0}", species);
                        //cs.Chromosome = c;
                        cs.AncestorRegions = new List<AncestorRegion>();

                        for (int k = 0; k < 10; k++)
                        {
                            AncestorRegion r = new AncestorRegion();
                            //r.ComparativeSpecies = cs;
                            r.Start = k * 10e6 + 100000 * x + j * 10000 + i * 500;
                            r.End = r.Start + (k + 1) * 1000000 + 100000 * x;
                            r.Label = string.Format("{0}", k);

                            ((List<AncestorRegion>)cs.AncestorRegions).Add(r);
                        }

                        ((List<ComparativeSpecies>)c.ComparativeSpecies).Add(cs);
                        x++;
                    }

                    ((List<Chromosome>)g.Chromosomes).Add(c);
                }

                genomes.Add(g);
            }

            return genomes;

            /*
            var genomes = new List<Genome>();

            for (int i = 1; i <=4; i++)
            {
                Genome g = new Genome();
                g.Name = "G" + i;
                g.Chromosomes = new List<Chromosome>();

                for (int j=1; j <= 5; j++)
                {
                    Chromosome c = new Chromosome();
                    c.Name = string.Format("Chr:{0}.{1}", i, j);
                    c.ComparativeSpecies = new List<ComparativeSpecies>();

                    int x = 0;
                    foreach (var species in new string[] { "dog", "mouse", "cat"})
                    {
                        ComparativeSpecies cs = new ComparativeSpecies();
                        cs.SpeciesName = string.Format("{0}:{1}", c.Name, species);
                        cs.AncestorRegions = new List<AncestorRegion>();

                        for (int k = 0; k < 10; k++)
                        {
                            AncestorRegion r = new AncestorRegion();
                            r.Start = k*100 + 10*x;
                            r.End = r.Start + (k+1)*10 + 10*x;
                            r.Label = string.Format("{0}{1}{2}", i, j, k);

                            ((List<AncestorRegion>)cs.AncestorRegions).Add(r);
                        }

                        ((List<ComparativeSpecies>)c.ComparativeSpecies).Add(cs);
                        x++;
                    }

                    ((List<Chromosome>)g.Chromosomes).Add(c);
                }
            
                genomes.Add(g);
            }

            return genomes;
             */
        }

        private void OnScaleValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (layoutTransformer == null) return;

            if (xbScaleLock.IsChecked == true)
            {
                sliderX.ValueChanged -= OnScaleValueChanged;
                sliderY.ValueChanged -= OnScaleValueChanged;

                if (ReferenceEquals(sender, sliderX))
                    sliderY.Value = sliderX.Value;
                else
                    sliderX.Value = sliderY.Value;

                sliderX.ValueChanged += OnScaleValueChanged;
                sliderY.ValueChanged += OnScaleValueChanged;
            }

            layoutTransformer.ApplyLayoutTransform();
        }

        private void OnResetScaleClick(object sender, System.Windows.RoutedEventArgs e)
        {
            sliderX.Value = sliderY.Value = 1;
            layoutTransformer.ApplyLayoutTransform();
        }

        private void OnDataScaleApplyClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = (Button) sender;
            btn.IsEnabled = false;
            txtDataScale.IsEnabled = false;

            ScaleConverter.Scale = Double.Parse(txtDataScale.Text);
            var data = genomesViewer.DataContext;
            genomesViewer.DataContext = null;
            genomesViewer.DataContext = data;

            btn.IsEnabled = true;
            txtDataScale.IsEnabled = true;
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

    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
