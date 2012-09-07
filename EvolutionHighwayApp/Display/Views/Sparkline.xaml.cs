using System.Collections.Generic;
using System.Windows;
using EvolutionHighwayApp.Display.ViewModels;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.Views
{
    public partial class Sparkline
    {
        public static readonly DependencyProperty DataPointsProperty =
            DependencyProperty.Register("DataPoints", typeof(List<FeatureDensity>), typeof(Sparkline),
            new PropertyMetadata((d, e) => ((Sparkline)d).ViewModel.DataPoints = (List<FeatureDensity>)e.NewValue));

        public List<FeatureDensity> DataPoints
        {
            get { return (List<FeatureDensity>) GetValue(DataPointsProperty); }
            set { SetValue(DataPointsProperty, value); }
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
