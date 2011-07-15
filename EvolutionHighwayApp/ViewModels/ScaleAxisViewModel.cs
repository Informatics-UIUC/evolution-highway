using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.ViewModels
{
    public class ScaleAxisViewModel : ModelBase, IDisposable
    {
        #region ViewModel Bindable Properties

        private int _minorTickMarks;
        public int MinorTickMarks
        {
            get { return _minorTickMarks; }
            set
            {
                if (MinorTickMarks == value) return;
                _minorTickMarks = value;

                NotifyPropertyChanged(() => MinorTickMarks);
                NotifyPropertyChanged(() => MinorTicks);
            }
        }

        private int _majorTickMarks;
        public int MajorTickMarks
        {
            get { return _majorTickMarks; }
            set
            {
                if (MajorTickMarks == value) return;
                _majorTickMarks = value;

                NotifyPropertyChanged(() => MajorTickMarks);
                NotifyPropertyChanged(() => MajorTicks);
            }
        }

        private double _maxScale;
        public double MaxScale
        {
            get { return _maxScale; }
            set
            {
                if (MaxScale == value) return;
                _maxScale = value;

                _height = Convert.ToInt32(Math.Floor((double)_scaleConverter.Convert(_maxScale, null, null, null)));

                NotifyPropertyChanged(() => MaxScale);
                NotifyPropertyChanged(() => MinorTicks);
                NotifyPropertyChanged(() => MajorTicks);
            }
        }

        public Geometry MinorTicks
        {
            get { return GetMinorTicksGeometry(); }
        }

        public Geometry MajorTicks
        {
            get { return GetMajorTicksGeometry(); }
        }

        #endregion

        private readonly IDisposable _displaySizeChangedObserver;
        private readonly ScaleConverter _scaleConverter;
        private int _height;

        public ScaleAxisViewModel()
        {
            _displaySizeChangedObserver = IoC.Container.Resolve<IEventPublisher>().GetEvent<DisplaySizeChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e =>
                               {
                                   _height = Convert.ToInt32(Math.Floor((double)_scaleConverter.Convert(MaxScale, null, null, null)));
                                   NotifyPropertyChanged(() => MinorTicks);
                                   NotifyPropertyChanged(() => MajorTicks);
                               });

            _scaleConverter = new ScaleConverter();
        }


        private Geometry GetMinorTicksGeometry()
        {
            if (_height == 0) return null;

            var gg = new GeometryGroup();
            for (var i = 0; i <= _height; i += MinorTickMarks)
            {
                if (i % MajorTickMarks == 0) continue;
                
                gg.Children.Add(new LineGeometry { StartPoint = new Point(7, i), EndPoint = new Point(20, i) });
            }

            return gg;
        }

        private Geometry GetMajorTicksGeometry()
        {
            if (_height == 0) return null;

            var gg = new GeometryGroup();
            for (var i = 0; i <= _height; i += MajorTickMarks)
                gg.Children.Add(new LineGeometry { StartPoint = new Point(0, i), EndPoint = new Point(20, i) });

            return gg;
        }

        public void Dispose()
        {
            _displaySizeChangedObserver.Dispose();
        }
    }
}
