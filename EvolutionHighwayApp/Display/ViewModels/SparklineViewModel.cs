using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Display.Controllers;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.ViewModels
{
    public class SparklineViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public AppSettings AppSettings { get; set; }

        private RefChromosome _refChromosome;
        public RefChromosome RefChromosome
        {
            get { return _refChromosome; }
            set
            {
                if (_refChromosome == value)
                    return;

                _refChromosome = value;
                Refresh();
            }
        }

        public List<FeatureDensity> DataPoints
        {
            get { return _refChromosome == null ? null : _refChromosome.AdjacencyScore; }
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

        public bool ShowAdjacencyScore
        {
            get { return _displayController.ShowFeatureDensitySparkline && DataPoints != null && !DataPoints.IsEmpty(); }
        }

        #endregion

        private static readonly ScaleConverter ScaleConverter = new ScaleConverter();
        private readonly IDisplayController _displayController;
        private readonly IDisposable _showAdjacencyScoreChangedObserver;
        private readonly IEventPublisher _eventPublisher;

        public SparklineViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();

            _displayController = IoC.Container.Resolve<IDisplayController>();
            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            _showAdjacencyScoreChangedObserver = _eventPublisher.GetEvent<ShowFeatureDensitySparklineEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => Refresh());
        }

        private void Refresh()
        {
            NotifyPropertyChanged(() => SparklinePointCollection);
            NotifyPropertyChanged(() => SparklineDataPoints);
            NotifyPropertyChanged(() => ShowAdjacencyScore);
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

        public override void Dispose()
        {
            base.Dispose();

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
