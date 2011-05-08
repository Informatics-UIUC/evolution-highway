using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EvolutionHighwayWidgets.Converters;

namespace EvolutionHighwayWidgets
{
    public partial class ScaleAxis : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty MinorTickMarksProperty =
            DependencyProperty.Register("MinorTickMarks", typeof (double), typeof (ScaleAxis), null);

        public static readonly DependencyProperty MajorTickMarksProperty =
            DependencyProperty.Register("MajorTickMarks", typeof(double), typeof(ScaleAxis), null);

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof (double), typeof (ScaleAxis), new PropertyMetadata(OnSizeChanged));

        public double MinorTickMarks
        {
            get { return (double) GetValue(MinorTickMarksProperty); }
            set { SetValue(MinorTickMarksProperty, value); }
        }

        public double MajorTickMarks
        {
            get { return (double) GetValue(MajorTickMarksProperty); }
            set { SetValue(MajorTickMarksProperty, value); }
        }

        public double Size
        {
            get { return (double) GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScaleAxis)d).OnPropertyChanged("MinorTicks");
            ((ScaleAxis)d).OnPropertyChanged("MajorTicks");
        }

        public Geometry MinorTicks
        {
            get
            {
                var converter = new ScaleConverter();
                var gg = new GeometryGroup();
                for (double i = 0; i <= Size; i += MinorTickMarks)
                {
                    if (i % MajorTickMarks == 0) continue;
                    var iScaled = (double)converter.Convert(i, null, null, null);
                    gg.Children.Add(new LineGeometry {StartPoint = new Point(7, iScaled), EndPoint = new Point(ActualWidth + 10, iScaled)});
                }
                return gg;
            }
        }

        public Geometry MajorTicks
        {
            get
            {
                var converter = new ScaleConverter();
                var gg = new GeometryGroup();
                for (double i = 0; i <= Size; i += MajorTickMarks)
                {
                    var iScaled = (double)converter.Convert(i, null, null, null);
                    gg.Children.Add(new LineGeometry { StartPoint = new Point(0, iScaled), EndPoint = new Point(ActualWidth + 10, iScaled) });
                }
                return gg;
            }
        }

        public ScaleAxis()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
