using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;
using EvolutionHighwayApp.Model;

namespace EvolutionHighwayApp.Views
{
    public partial class SynBlocks : Page
    {

        private IEnumerable<Genome> data = GetFakeGenomes();

        public SynBlocks()
        {
            InitializeComponent();

            lstGenomes.ItemsSource = from genome in data select genome.Name;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            accordion.SelectAll();
        }

        private void lstGenomes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var result = (from genome in data
                          where lstGenomes.SelectedItems.Contains(genome.Name)
                          from chr in genome.Chromosomes
                          select chr.Name).Distinct();

            lstChromosomes.ItemsSource = result;
        }

        private void lstChromosomes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var result = (from genome in data
                          where lstGenomes.SelectedItems.Contains(genome.Name)
                          from chr in genome.Chromosomes
                          where lstChromosomes.SelectedItems.Contains(chr.Name)
                          from species in chr.ComparativeSpecies
                          select species.SpeciesName).Distinct();

            lstSpecies.ItemsSource = result;
        }

        private void lstSpecies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var result = from genome in data
                         where lstGenomes.SelectedItems.Contains(genome.Name)
                         select genome;
            genomeViewer.DataContext = result;
        }



        public static IEnumerable<Genome> GetFakeGenomes()
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
                    c.ComparativeSpecies = new List<ComparativeSpecies>();

                    int x = 0;
                    foreach (var species in new string[] { "dog", "mouse", "cat" })
                    {
                        ComparativeSpecies cs = new ComparativeSpecies();
                        cs.SpeciesName = string.Format("{0}", species);
                        cs.AncestorRegions = new List<AncestorRegion>();

                        for (int k = 0; k < 10; k++)
                        {
                            AncestorRegion r = new AncestorRegion();
                            r.Start = k * 100 + 10 * x + j * 10 + i * 50;
                            r.End = r.Start + (k + 1) * 10 + 10 * x;
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
    }
}
