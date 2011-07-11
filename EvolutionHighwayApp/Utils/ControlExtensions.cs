using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EvolutionHighwayApp.Utils
{
    public static class ControlExtensions
    {
        public static T FindVisualParent<T>(FrameworkElement target) where T : FrameworkElement
        {
            if (target == null) return null;

            var visParent = VisualTreeHelper.GetParent(target);
            var result = visParent as T;
            
            return result ?? FindVisualParent<T>(visParent as FrameworkElement);
        }

        public static T GetTemplateChild<T>(this Control target, string templatePartName) where T : FrameworkElement
        {
            var childCount = VisualTreeHelper.GetChildrenCount(target);
            if (childCount == 0) return null;

            return ((FrameworkElement) VisualTreeHelper.GetChild(target, 0)).FindName(templatePartName) as T;
        }
    }
}
