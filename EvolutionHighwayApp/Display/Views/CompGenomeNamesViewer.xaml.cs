using System.Collections.Generic;
using System.Windows;
using EvolutionHighwayApp.Display.ViewModels;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Display.Views
{
    public partial class CompGenomeNamesViewer
    {
        public static readonly DependencyProperty CompGenomesProperty =
            DependencyProperty.Register("CompGenomes", typeof(IEnumerable<CompGenome>), typeof(CompGenomeNamesViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((CompGenomeNamesViewer)o).ViewModel.CompGenomes = (IEnumerable<CompGenome>)e.NewValue)));

        public static readonly DependencyProperty NamesWidthProperty =
            DependencyProperty.Register("NamesWidth", typeof(int), typeof(CompGenomeNamesViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((CompGenomeNamesViewer)o).ViewModel.NamesWidth = (int)e.NewValue)));

        public static readonly DependencyProperty NamesAlignmentProperty =
            DependencyProperty.Register("NamesAlignment", typeof(VerticalAlignment), typeof(CompGenomeNamesViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((CompGenomeNamesViewer)o).ViewModel.NamesAlignment = (VerticalAlignment)e.NewValue)));

        public IEnumerable<CompGenome> CompGenomes
        {
            get { return (IEnumerable<CompGenome>)GetValue(CompGenomesProperty); }
            set { SetValue(CompGenomesProperty, value); }
        }

        public int NamesWidth
        {
            get { return (int)GetValue(NamesWidthProperty); }
            set { SetValue(NamesWidthProperty, value); }
        }

        public VerticalAlignment NamesAlignment
        {
            get { return (VerticalAlignment)GetValue(NamesAlignmentProperty); }
            set { SetValue(NamesAlignmentProperty, value); }
        }

        private CompGenomeNamesViewModel ViewModel
        {
            get { return DataContext as CompGenomeNamesViewModel; }
        }

        public CompGenomeNamesViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new CompGenomeNamesViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }
    }
}
