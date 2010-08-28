using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows.Controls;
using System.Windows.Navigation;
using EvolutionHighwayModel;

namespace EvolutionHighwayApp.Views
{
    public partial class SynBlocks : Page
    {
        private readonly IEHDataService _serviceProxy = 
            EHDataService.CreateServiceProxy(new EndpointAddress("http://leovip027.ncsa.uiuc.edu:8080"));

        public SynBlocks()
        {
            InitializeComponent();

            _serviceProxy.BeginListGenomes(
                delegate(IAsyncResult asyncResult)
                {
                    var genomes = _serviceProxy.EndListGenomes(asyncResult);
                    Dispatcher.BeginInvoke(
                        delegate
                        {
                            lstGenomes.ItemsSource = genomes;
                        }
                    );
                }
            );

            _serviceProxy.BeginListSynblocks("HSA", "1", "dog",
                delegate(IAsyncResult asyncResult)
                    {
                        var synBlocks = _serviceProxy.EndListSynblocks(asyncResult);
                        Dispatcher.BeginInvoke(
                            delegate
                                {
                                    genomeViewer.DataContext = new Genome[]
                                                                   {
                                                                       new Genome()
                                                                           {
                                                                               Name = "HSA",
                                                                               Chromosomes = new Chromosome[]
                                                                                                 {
                                                                                                     new Chromosome
                                                                                                         {
                                                                                                             Name = "1",
                                                                                                             ComparativeSpecies
                                                                                                                 =
                                                                                                                 new ComparativeSpecies
                                                                                                                 []
                                                                                                                     {
                                                                                                                         new ComparativeSpecies
                                                                                                                             {
                                                                                                                                 SpeciesName
                                                                                                                                     =
                                                                                                                                     "dog",
                                                                                                                                 AncestorRegions
                                                                                                                                     =
                                                                                                                                     synBlocks
                                                                                                                             }
                                                                                                                     }
                                                                                                         }
                                                                                                 }
                                                                           }
                                                                   };
                                }
                        );
                    }
            );
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            accordion.SelectAll();
        }

        private void lstGenomes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var genome in lstGenomes.SelectedItems.Cast<Genome>())
            {
                _serviceProxy.BeginListChromosomes(genome.Name,
                    delegate(IAsyncResult asyncResult)
                        {
                            var chromosomes = _serviceProxy.EndListChromosomes(asyncResult);
                            Dispatcher.BeginInvoke(
                                delegate
                                {
                                    foreach (var chromosome in chromosomes)
                                        lstChromosomes.Items.Add(chromosome);
                                }
                            );
                        }
                );
            }
        }

        private void lstChromosomes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var result = (from genome in lstGenomes.SelectedItems.Cast<Genome>()
                          from chr in genome.Chromosomes
                          where lstChromosomes.SelectedItems.Contains(chr.Name)
                          from species in chr.ComparativeSpecies
                          select species.SpeciesName).Distinct();

            lstSpecies.ItemsSource = result;
        }

        private void lstSpecies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
//            var result = from genome in data
//                         where lstGenomes.SelectedItems.Contains(genome.Name)
//                         select genome;
//            genomeViewer.DataContext = result;
        }
    }
}
