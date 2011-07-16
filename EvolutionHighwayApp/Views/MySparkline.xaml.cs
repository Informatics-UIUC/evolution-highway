using System.Collections.Generic;
using System.Windows;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class MySparkline
    {
        public static readonly DependencyProperty DataPointsProperty =
            DependencyProperty.Register("DataPoints", typeof(List<FeatureDensity>), typeof(MySparkline),
            new PropertyMetadata((d, e) => ((MySparkline)d).ViewModel.DataPoints = (List<FeatureDensity>)e.NewValue));

        public List<FeatureDensity> DataPoints
        {
            get { return (List<FeatureDensity>) GetValue(DataPointsProperty); }
            set { SetValue(DataPointsProperty, value); }
        }

        private MySparklineViewModel ViewModel
        {
            get { return DataContext as MySparklineViewModel; }
        }

        public MySparkline()
        {
            InitializeComponent();

            DataContext = new MySparklineViewModel();

            SizeChanged += (o, e) => ViewModel.Size = e.NewSize;
        }
    }
}
