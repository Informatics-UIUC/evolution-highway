using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.ViewModels
{
    public class SparklineViewModel : ModelBase, IDisposable
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
                NotifyPropertyChanged(() => ShowAdjacencyScore);
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

        private bool _showAdjacencyScore;
        public bool ShowAdjacencyScore
        {
            get { return _showAdjacencyScore && DataPoints != null && !DataPoints.IsEmpty(); }
            set { NotifyPropertyChanged(() => ShowAdjacencyScore, ref _showAdjacencyScore, value); }
        }

        #endregion

        private static readonly ScaleConverter ScaleConverter = new ScaleConverter();
        private readonly IDisposable _showAdjacencyScoreChangedObserver;

        public SparklineViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();
            ShowAdjacencyScore = AppSettings.ShowAdjacencyScore;

            _showAdjacencyScoreChangedObserver = IoC.Container.Resolve<IEventPublisher>().GetEvent<ShowAdjacencyScoreEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => ShowAdjacencyScore = e.ShowAdjacencyScore);
        }

        private PointCollection GetSparklinePointCollection()
        {
            if (DataPoints == null) return null;

            var points = new PointCollection();

            for (var i = 0; i < DataPoints.Count; i++)
            {
                var dataPoint = DataPoints[i];
                var scaledStart = (double) ScaleConverter.Convert(dataPoint.RefStart, null, null, null);
                var scaledEnd = (double) ScaleConverter.Convert(dataPoint.RefEnd, null, null, null);
                var scoreOffset = dataPoint.Score*Size.Width;

                if (i == 0)
                    points.Add(new Point(0, scaledStart));

                points.Add(new Point(scoreOffset, scaledStart));
                if (dataPoint.RefStart != dataPoint.RefEnd)
                    points.Add(new Point(scoreOffset, scaledEnd));

                if (i == DataPoints.Count - 1)
                    points.Add(new Point(0, Math.Max(scaledStart, scaledEnd)));
            }

            return points;
        }

        public void Dispose()
        {
            _showAdjacencyScoreChangedObserver.Dispose();
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
            var adjustment = double.Parse(parameter.ToString());

            return (double) value + adjustment;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
