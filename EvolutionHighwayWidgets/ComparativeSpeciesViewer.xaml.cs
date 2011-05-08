using System.Windows;
using System.Windows.Controls;
using EvolutionHighwayModel;

namespace EvolutionHighwayWidgets
{
    public partial class ComparativeSpeciesViewer : UserControl
    {
        public static readonly DependencyProperty ChromosomeProperty =
            DependencyProperty.Register("Chromosome", typeof(Chromosome), typeof(ComparativeSpeciesViewer), null);

        public Chromosome Chromosome
        {
            get { return (Chromosome) GetValue(ChromosomeProperty); }
            set { SetValue(ChromosomeProperty, value); }
        }

        public ComparativeSpeciesViewer()
        {
            InitializeComponent();
        }
    }
}
