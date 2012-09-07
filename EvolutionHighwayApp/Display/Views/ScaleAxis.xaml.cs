using System.Windows;
using EvolutionHighwayApp.Display.ViewModels;

namespace EvolutionHighwayApp.Display.Views
{
    public partial class ScaleAxis
    {
        public static readonly DependencyProperty MinorTickMarksProperty =
            DependencyProperty.Register("MinorTickMarks", typeof(int), typeof(ScaleAxis),
            new PropertyMetadata((d, e) => ((ScaleAxis) d).ViewModel.MinorTickMarks = (int) e.NewValue));

        public static readonly DependencyProperty MajorTickMarksProperty =
            DependencyProperty.Register("MajorTickMarks", typeof(int), typeof(ScaleAxis),
            new PropertyMetadata((d, e) => ((ScaleAxis)d).ViewModel.MajorTickMarks = (int) e.NewValue));

        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register("MaxScale", typeof(double), typeof(ScaleAxis), 
            new PropertyMetadata((d, e) => ((ScaleAxis) d).ViewModel.MaxScale = (double) e.NewValue));

        public int MinorTickMarks
        {
            get { return (int) GetValue(MinorTickMarksProperty); }
            set { SetValue(MinorTickMarksProperty, value); }
        }

        public int MajorTickMarks
        {
            get { return (int) GetValue(MajorTickMarksProperty); }
            set { SetValue(MajorTickMarksProperty, value); }
        }

        public double MaxScale
        {
            get { return (double) GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        private ScaleAxisViewModel ViewModel
        {
            get { return DataContext as ScaleAxisViewModel; }
        }

        public ScaleAxis()
        {
            InitializeComponent();

            DataContext = new ScaleAxisViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }
    }
}
