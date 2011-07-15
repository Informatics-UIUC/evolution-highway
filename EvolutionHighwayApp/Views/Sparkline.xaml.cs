using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Markup;

using EvolutionHighwayApp.Models;


namespace EvolutionHighwayApp.Views
{
    public partial class Sparkline : UserControl
    {
        private Polyline _polyline;
        private Point? _latestAddedPoint;
        private double _my_width = 0.0f;
        private double _my_height = 0.0f;
        //private double _start_buffer = 1.0f;
       // private double _end_buffer = 7.0f;
        private double _hack_height = 0.0f;
        private double _last_w = 0;
        private double _last_h = 0;
        private double _point_size = 2.250f;
        private double _polyline_thickness = 0.5f;
        private double _refEnd_length = 1.0f;


        #region ITEM SOURCE HANDLER
        internal class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
        {
            /// <summary>
            /// WeakReference to the instance listening for the event.
            /// </summary>
            private WeakReference _weakInstance;

            /// <summary>
            /// Gets or sets the method to call when the event fires.
            /// </summary>
            public Action<TInstance, TSource, TEventArgs> OnEventAction { get; set; }

            /// <summary>
            /// Gets or sets the method to call when detaching from the event.
            /// </summary>
            public Action<WeakEventListener<TInstance, TSource, TEventArgs>> OnDetachAction { get; set; }

            /// <summary>
            /// Initializes a new instances of the WeakEventListener class.
            /// </summary>
            /// <param name="instance">Instance subscribing to the event.</param>
            public WeakEventListener(TInstance instance)
            {
                if (null == instance)
                {
                    throw new ArgumentNullException("instance");
                }
                _weakInstance = new WeakReference(instance);
            }

            /// <summary>
            /// Handler for the subscribed event calls OnEventAction to handle it.
            /// </summary>
            /// <param name="source">Event source.</param>
            /// <param name="eventArgs">Event arguments.</param>
            public void OnEvent(TSource source, TEventArgs eventArgs)
            {
                TInstance target = (TInstance)_weakInstance.Target;
                if (null != target)
                {
                    // Call registered action
                    if (null != OnEventAction)
                    {
                        OnEventAction(target, source, eventArgs);
                    }
                }
                else
                {
                    // Detach from event
                    Detach();
                }
            }

            /// <summary>
            /// Detaches from the subscribed event.
            /// </summary>
            public void Detach()
            {
                if (null != OnDetachAction)
                {
                    OnDetachAction(this);
                    OnDetachAction = null;
                }
            }
        }

        private WeakEventListener<Sparkline, object, NotifyCollectionChangedEventArgs> _weakEventListener;

        public static DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Sparkline), new PropertyMetadata(OnItemsSourcePropertyChanged));
        
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sparkline sparky = d as Sparkline;
            if (sparky != null)
            {
                IEnumerable oldValue = e.OldValue as IEnumerable;
                IEnumerable newValue = e.NewValue as IEnumerable;
                sparky.OnItemsSourcePropertyChanged(oldValue, newValue);
            }
        }
 
        protected virtual void OnItemsSourcePropertyChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            // Remove handler for oldValue.CollectionChanged (if present)
            INotifyCollectionChanged oldValueINotifyCollectionChanged = oldValue as INotifyCollectionChanged;
            if (null != oldValueINotifyCollectionChanged)
            {
                // Detach the WeakEventListener
                if (null != _weakEventListener)
                {
                    _weakEventListener.Detach();
                    _weakEventListener = null;
                }
            }

            // Add handler for newValue.CollectionChanged (if possible)
            INotifyCollectionChanged newValueINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (null != newValueINotifyCollectionChanged)
            {
                // Use a WeakEventListener so that the backwards reference doesn't keep this object alive
                _weakEventListener = new WeakEventListener<Sparkline, object, NotifyCollectionChangedEventArgs>(this);
                _weakEventListener.OnEventAction = (instance, source, eventArgs) => instance.ItemsSourceCollectionChanged(source, eventArgs);
                _weakEventListener.OnDetachAction = (weakEventListener) => newValueINotifyCollectionChanged.CollectionChanged -= weakEventListener.OnEvent;
                newValueINotifyCollectionChanged.CollectionChanged += _weakEventListener.OnEvent;
            }

            // Handle property change
            RebuildSparkline();
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildSparkline();
        }

        public void RebuildSparkline()
        {
            try
            {
                var test = (List<FeatureDensity>)ItemsSource;
                var test2 = from p in test orderby p.RefStart ascending select p;
                double min2 = (from obj2 in (List<FeatureDensity>)test select obj2.RefStart).Min();
                double max2 = (from obj2 in (List<FeatureDensity>)test select obj2.RefEnd).Max();

                double length2 = max2; // max2 - min2;
                _refEnd_length = max2; // length2;
                
                ObjectSeries.Clear();
                //KEEP? ObjectSeries.Add(new ObjectValue() { RefStart = 0, Length = 0, Score = 0, Tooltip = "" });

                foreach (FeatureDensity fd in test2)
                {
                     ObjectSeries.AddObjectValue(fd.Score, fd.RefStart, length2, fd.ToolTip);
                }

                ResetObjectSeries();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Sparkline RebuildSparkline: {0}", ex.Message);
            }

        }

        #endregion

        

        public static DependencyProperty ObjectSeriesProperty = DependencyProperty.Register("ObjectSeries", typeof(ObjectSeries), typeof(Sparkline), new PropertyMetadata(new ObjectSeries(), OnObjectSeriesPropertyChanged));
        public static DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(Sparkline), new PropertyMetadata(0.5));
        public static DependencyProperty LineMarginProperty = DependencyProperty.Register("LineMargin", typeof(Thickness), typeof(Sparkline), new PropertyMetadata(new Thickness(0)));
        public static DependencyProperty PointFillProperty = DependencyProperty.Register("PointFill", typeof(Brush), typeof(Sparkline), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));
        public static DependencyProperty PointRadiusProperty = DependencyProperty.Register("PointRadius", typeof(double), typeof(Sparkline), new PropertyMetadata(0.0));

        public ObjectSeries ObjectSeries
        {
            get { return (ObjectSeries)GetValue(ObjectSeriesProperty); }
            set { SetValue(ObjectSeriesProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Thickness LineMargin
        {
            get { return (Thickness)GetValue(LineMarginProperty); }
            set { SetValue(LineMarginProperty, value); }
        }

        public Brush PointFill
        {
            get { return (Brush)GetValue(PointFillProperty); }
            set { SetValue(PointFillProperty, value); }
        }

        public double PointRadius
        {
            get { return (double)GetValue(PointRadiusProperty); }
            set { SetValue(PointRadiusProperty, value); }
        }

        public class ObjectValueAddedEventArgs : EventArgs
        {
            public Point Point { get; set; }
            public Panel Panel { get; set; }
        }

        public delegate void ObjectValueAddedHandler(Sparkline obj, ObjectValueAddedEventArgs eventArgs);

        public event ObjectValueAddedHandler ObjectValueAdded;

        protected void OnObjectValueAdded(Point po, Panel pa)
        {
            var handler = ObjectValueAdded;
            if (handler != null)
            {
                handler(this, new ObjectValueAddedEventArgs { Point = po, Panel = pa });
            }
        }

        public Sparkline()
        {
            InitializeComponent();

            ObjectSeries = new ObjectSeries();
            //ObjectSeries.Add(new ObjectValue() { RefStart = 0, Length = 0, Score = 0, Tooltip = "" });
            Canvas.SizeChanged += new SizeChangedEventHandler(Canvas_SizeChanged);

            _visibility = System.Windows.Visibility.Visible;
            InitializePolyline();
            PointRadius = _point_size;
            
        }

        void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetObjectSeries();

        }

        //FIX ME (I KNOW SILLY HACK)
        public void SetWidthHeight(double w, double h, double hack_h)
        {         
            Canvas.Width = w;
            Canvas.Height = h;
            _hack_height = hack_h;
            _my_width = w;
            _my_height = h;// -50.0f; // -_start_buffer - _end_buffer;
            ResetObjectSeries();
        }

        private void InitializePolyline()
        {
            _polyline = new Polyline();
            if (Foreground == null)
                Foreground = new SolidColorBrush(Colors.White);

            BindingOperations.SetBinding(_polyline, Shape.StrokeProperty, new Binding("Foreground") { Mode = BindingMode.TwoWay, Source = this });
            BindingOperations.SetBinding(_polyline, Shape.StrokeThicknessProperty, new Binding("StrokeThickness") { Mode = BindingMode.TwoWay, Source = this });
            BindingOperations.SetBinding(_polyline, MarginProperty, new Binding("LineMargin") { Mode = BindingMode.TwoWay, Source = this });
            Canvas.Children.Add(_polyline);
        }

        private static void OnObjectSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Sparkline)d).OnObjectSeriesPropertyChanged(e);
        }

        private void OnObjectSeriesPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null) ((ObjectSeries)e.OldValue).CollectionChanged -= ObjectSeriesCollectionChanged;
            if (e.NewValue != null) ((ObjectSeries)e.NewValue).CollectionChanged += ObjectSeriesCollectionChanged;
        }

        private void ObjectSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    //KEEP foreach (var timeValue in e.NewItems.OfType<ObjectValue>()) DrawObjectValue(timeValue);
                    break;           
                default:
                    ResetObjectSeries();
                    break;
            }
        }
     
        //FIX ME
        private void ResetObjectSeries()
        {
            try
            {
                _my_width = Canvas.Width;
                _my_height = Canvas.Height;


                if (_my_width == 0 || _my_height == 0)
                    return;

                //if (_my_width == _last_w || _my_height == _last_h)
                //    return;

                if (_my_height == _last_h)
                    return;

                _last_w = _my_width;
                _last_h = _my_height;

                InitializePolyline();

                //Canvas.Width = _my_width;
                //Canvas.Height = _my_height;
                Canvas.Children.Clear();
                Canvas.Children.Add(_polyline);

                //_refEnd_length = (from obj2 in (ObjectSeries) select obj2.Length).Max();

                foreach (var timeValue in ObjectSeries)
                {
                    DrawObjectValue(timeValue);
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Sparkline ResetObjectSeries: {0}", ex.Message);
            }
        }

        private void DrawObjectValue(ObjectValue value)
        {
            var point = GetPoint(value);
            AddPoint(point, value.Tooltip);
        }
 
        private void AddPoint(Point point, String tool_tip)
        {
            try
            {              
                _polyline.Points.Add(point);
                _latestAddedPoint = point;
                _polyline.Stroke = new SolidColorBrush(Colors.Black);
                _polyline.StrokeThickness = _polyline_thickness;
                
                if (PointRadius > 0.0)
                    Canvas.Children.Add(DrawDot(point, tool_tip));

                OnObjectValueAdded(point, Canvas);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Sparkline AddPoint: {0}", ex.Message);
            }
        }

        private Path DrawDot(Point center, String tool_tip)
        {
            var path = new Path();

            var circle = new EllipseGeometry { Center = center, RadiusX = PointRadius, RadiusY = PointRadius };

            path.Fill = PointFill;
            path.Stroke = new SolidColorBrush(Colors.Black);
            path.Data = circle;

            ToolTip tt = new ToolTip
            {
                Content = tool_tip
            };

            ToolTipService.SetToolTip(path, tt);
            return path;
        }
      
        //FIX ME
        private Point GetPoint(ObjectValue timeValue)
        {

            double x_pos = timeValue.Score * _my_width;
            x_pos -= (1.0f * PointRadius);
            //double y_pos = (timeValue.RefStart / timeValue.Length) * _my_height;
            double y_pos = (timeValue.RefStart / _refEnd_length) * _my_height;

            System.Diagnostics.Debug.WriteLine("Sparkline GetPoint: {0} ", _my_height);
            //y_pos += _start_buffer;
            //y_pos -= _hack_height;
            y_pos -= (1.0f * PointRadius);
            //System.Diagnostics.Debug.WriteLine("Sparkline GetPoint: {0} {1} {2} {3} {4} ", x_pos, y_pos, timeValue.RefStart, timeValue.Length, _refEnd_length);

            return new Point(x_pos,y_pos);          
        }

        private Visibility _visibility;
        public event EventHandler VisibilityChanged;
        protected virtual void OnVisibilityChanged()
        {
            base.Visibility = this.Visibility;
            if (_visibility == Visibility.Visible)
            {
                ResetObjectSeries();
            }

            if (this.VisibilityChanged != null)
                this.VisibilityChanged(this, new EventArgs());
        }
    }

    public class ObjectValue
    {       
        public String Tooltip;
        public double Score;
        public double Length;
        public double RefStart;
    }

    public class ObjectSeries : ObservableCollection<ObjectValue>
    {
        public void AddObjectValue(double score, double start, double len, String tooltip)
        {
            try
            {
                Add(new ObjectValue {  Score = score,  RefStart = start,  Length = len, Tooltip = tooltip });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Sparkline AddObjectValue: {0}", ex.Message);
            }
        }
    }

    public class YCoordinateToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Thickness(0, (double?)value ?? 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

