using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.ViewModels
{
    public class MySparklineViewModel : ModelBase
    {
        #region ViewModel Bindable Properties

        public AppSettings AppSettings { get; set; }

        private List<FeatureDensity> _dataPoints;
        public List<FeatureDensity> DataPoints
        {
            get { return _dataPoints; }
            set
            {
                NotifyPropertyChanged(() => SparklinePointCollection, ref _dataPoints, value);
                NotifyPropertyChanged(() => SparklineDataPoints);
            }
        }

        private Size _size;
        public Size Size
        {
            get { return _size; }
            set
            {
                if (Size == value) return;
                _size = value;

                NotifyPropertyChanged(() => SparklinePointCollection);
                NotifyPropertyChanged(() => SparklineDataPoints);
            }
        }

        public PointCollection SparklinePointCollection
        {
            get { return GetSparklinePointCollection(); }
        }

        public IEnumerable<FeatureDensityDataPoint> SparklineDataPoints
        {
            get { return DataPoints == null ? null : DataPoints.Select(data => new FeatureDensityDataPoint(data, Size.Width)); }
        }

        #endregion

        private static readonly ScaleConverter ScaleConverter = new ScaleConverter();

        public MySparklineViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();
        }

        private PointCollection GetSparklinePointCollection()
        {
            if (DataPoints == null) return null;

            var points = new PointCollection {new Point(0, 0)};

            foreach (var dataPoint in DataPoints)
            {
                var scaledStart = (double) ScaleConverter.Convert(dataPoint.RefStart, null, null, null);
                var scaledEnd = (double) ScaleConverter.Convert(dataPoint.RefEnd, null, null, null);
                var scoreOffset = dataPoint.Score*Size.Width;

                points.Add(new Point(scoreOffset, scaledStart));
                if (dataPoint.RefStart != dataPoint.RefEnd)
                    points.Add(new Point(scoreOffset, scaledEnd));
            }

            points.Add(new Point(0, Size.Height));

            return points;
        }

        public class FeatureDensityDataPoint
        {
            public double X
            {
                get { return FeatureDensityData.Score * Width; }
            }

            public double Y
            {
                get { return (double) ScaleConverter.Convert(FeatureDensityData.RefStart, null, null, null); }
            }

            public FeatureDensity FeatureDensityData { get; private set; }
            private double Width { get; set; }

            public FeatureDensityDataPoint(FeatureDensity data, double width)
            {
                FeatureDensityData = data;
                Width = width;
            }
        }
    }

    // Needed for adjusting the center of the ellipse to match the data point
    public class OffsetAdjustmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var adjustment = int.Parse(parameter.ToString());

            return (double) value + adjustment;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
