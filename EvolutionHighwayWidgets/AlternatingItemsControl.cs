using System.Windows;
using System.Windows.Controls;

namespace EvolutionHighwayWidgets
{
    public class AlternatingItemsControl: ItemsControl
    {
        public int AlternationIndex { get; private set; }

        public static readonly DependencyProperty AlternationCountProperty =
            DependencyProperty.Register("AlternationCount", typeof (int), typeof (AlternatingItemsControl), null);

        public int AlternationCount
        {
            get { return (int)GetValue(AlternationCountProperty); }
            set { SetValue(AlternationCountProperty, value); }
        }

        public AlternatingItemsControl()
        {
            AlternationIndex = -1;
            AlternationCount = 2;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            AlternationIndex = -1;
            base.ClearContainerForItemOverride(element, item);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            AlternationIndex = (AlternationIndex + 1) % AlternationCount;
            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
