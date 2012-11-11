using System.Windows;
using EvolutionHighwayApp.Display.ViewModels;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.Views
{
    public partial class Sparkline
    {
        public static readonly DependencyProperty RefChromosomeProperty =
            DependencyProperty.Register("RefChromosome", typeof(RefChromosome), typeof(Sparkline), new PropertyMetadata(
                (o, e) => ((Sparkline)o).ViewModel.RefChromosome = (RefChromosome)e.NewValue));

        public RefChromosome RefChromosome
        {
            get { return (RefChromosome)GetValue(RefChromosomeProperty); }
            set { SetValue(RefChromosomeProperty, value); }
        }

        private SparklineViewModel ViewModel
        {
            get { return DataContext as SparklineViewModel; }
        }

        public Sparkline()
        {
            InitializeComponent();

            DataContext = new SparklineViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };

            SizeChanged += (o, e) => ViewModel.Size = e.NewSize;
        }
    }
}
