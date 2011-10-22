﻿using System.Windows;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class HighlightRegionCollectionViewer
    {
        public static readonly DependencyProperty RefChromosomeProperty =
            DependencyProperty.Register("RefChromosome", typeof(RefChromosome), typeof(HighlightRegionCollectionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((HighlightRegionCollectionViewer)o).ViewModel.RefChromosome = (RefChromosome)e.NewValue)));

        public RefChromosome RefChromosome
        {
            get { return (RefChromosome)GetValue(RefChromosomeProperty); }
            set { SetValue(RefChromosomeProperty, value); }
        }

        private HighlightRegionCollectionViewModel ViewModel
        {
            get { return DataContext as HighlightRegionCollectionViewModel; }
        }

        public HighlightRegionCollectionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new HighlightRegionCollectionViewModel();
        }
    }
}
