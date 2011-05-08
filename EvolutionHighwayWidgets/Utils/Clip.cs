// The original code for ClipToBounds came from Colin Eberhardt: http://www.codeproject.com/Articles/36495/Silverlight-ClipToBounds-Can-I-Clip-It-Yes-You-Can.aspx
// RadiusX, RadiusY, and other additions are changes were done by Dan Vanderboom: http://dvanderboom.wordpress.com/

using System.Windows;
using System.Windows.Media;

namespace EvolutionHighwayWidgets.Utils
{
	public class Clip
	{
		// ToBounds

		public static readonly DependencyProperty ToBoundsProperty =
			DependencyProperty.RegisterAttached("ToBounds", typeof(bool),
			typeof(Clip), new PropertyMetadata(false, OnToBoundsPropertyChanged));

		private static void OnToBoundsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var fe = obj as FrameworkElement;
		    if (fe == null) return;

		    ClipToBounds(fe);

		    // whenever the element which this property is attached to is loaded
		    // or re-sizes, we need to update its clipping geometry
		    fe.Loaded -= Element_Loaded;
		    fe.Loaded += Element_Loaded;
		    fe.SizeChanged -= Element_SizeChanged;
		    fe.SizeChanged += Element_SizeChanged;
		}

		public static bool GetToBounds(DependencyObject obj)
		{
			return (bool)obj.GetValue(ToBoundsProperty);
		}

		public static void SetToBounds(DependencyObject obj, bool clipToBounds)
		{
			obj.SetValue(ToBoundsProperty, clipToBounds);
		}

		// RadiusX

		public static readonly DependencyProperty RadiusXProperty =
			DependencyProperty.RegisterAttached("RadiusX", typeof(double),
			typeof(Clip), new PropertyMetadata(0.0, OnRadiusXPropertyChanged));

		private static void OnRadiusXPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var fe = obj as FrameworkElement;
			if (fe != null)
				ClipToBounds(fe);
		}

		public static double GetRadiusX(DependencyObject obj)
		{
			return (double)obj.GetValue(RadiusXProperty);
		}

		public static void SetRadiusX(DependencyObject obj, double RadiusX)
		{
			obj.SetValue(RadiusXProperty, RadiusX);
		}

		// RadiusY

		public static readonly DependencyProperty RadiusYProperty =
			DependencyProperty.RegisterAttached("RadiusY", typeof(double),
			typeof(Clip), new PropertyMetadata(0.0, OnRadiusYPropertyChanged));

		private static void OnRadiusYPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var fe = obj as FrameworkElement;
			if (fe != null)
				ClipToBounds(fe);
		}

		public static double GetRadiusY(DependencyObject obj)
		{
			return (double)obj.GetValue(RadiusYProperty);
		}

		public static void SetRadiusY(DependencyObject obj, double RadiusY)
		{
			obj.SetValue(RadiusYProperty, RadiusY);
		}

		// implementation

		private static void ClipToBounds(FrameworkElement Element)
		{
			if (GetToBounds(Element))
			{
				Element.Clip = new RectangleGeometry()
				{
					Rect = new Rect(0, 0, Element.ActualWidth, Element.ActualHeight)
				};

				var rect = Element.Clip as RectangleGeometry;
				rect.RadiusX = GetRadiusX(Element);
				rect.RadiusY = GetRadiusY(Element);
			}
			else
			{
				Element.Clip = null;
			}
		}

		static void Element_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ClipToBounds(sender as FrameworkElement);
		}

		static void Element_Loaded(object sender, RoutedEventArgs e)
		{
			ClipToBounds(sender as FrameworkElement);
		}
	}
}