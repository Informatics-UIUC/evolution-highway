﻿using System.Windows;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class CentromereRegionCollectionViewer
    {
        public static readonly DependencyProperty RefChromosomeProperty =
            DependencyProperty.Register("RefChromosome", typeof(RefChromosome), typeof(CentromereRegionCollectionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((CentromereRegionCollectionViewer)o).ViewModel.RefChromosome = (RefChromosome)e.NewValue)));

        public RefChromosome RefChromosome
        {
            get { return (RefChromosome)GetValue(RefChromosomeProperty); }
            set { SetValue(RefChromosomeProperty, value); }
        }

        private CentromereRegionCollectionViewModel ViewModel
        {
            get { return DataContext as CentromereRegionCollectionViewModel; }
        }

        public CentromereRegionCollectionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new CentromereRegionCollectionViewModel();
        }
    }
}
