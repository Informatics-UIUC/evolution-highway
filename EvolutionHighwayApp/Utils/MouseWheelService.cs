using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Browser;
using System.Windows.Input;
using System.Windows.Media;

namespace EvolutionHighwayApp.Utils
{
    public static class MouseWheelService
    {
        private static Point _currentPoint;
        private static UIElement _rootElement;

        static MouseWheelService()
        {
            // register events (for differet browsers)
            HtmlPage.Window.AttachEvent("DOMMouseScroll", OnWheelTurned);
            HtmlPage.Window.AttachEvent("onmousewheel", OnWheelTurned);
            HtmlPage.Document.AttachEvent("onmousewheel", OnWheelTurned);
        }

        public static void Enable(UIElement rootElement)
        {
            // prevent null reference exception
            if (rootElement == null) return;
            // only one root element possible with this implementation
            if (_rootElement != null) throw new InvalidOperationException("This method can only be called once");

            _rootElement = rootElement;
            _rootElement.MouseMove += OnMouseMove;
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            _currentPoint = e.GetPosition(null);
        }

        private static void OnWheelTurned(object sender, HtmlEventArgs args)
        {
            double delta = 0;
            var e = args.EventObject;

            if (e.GetProperty("wheelDelta") != null) // IE and Opera
            {
                delta = ((double) e.GetProperty("wheelDelta"));
                if (HtmlPage.Window.GetProperty("opera") != null)
                    delta = -delta;
            }
            else
                if (e.GetProperty("detail") != null) // Mozilla and Safari
                {
                    delta = -((double)e.GetProperty("detail"));
                }

            if (delta == 0) return;

            args.PreventDefault();
            e.SetProperty("returnValue", false);

            // go through all element beneath the current mouse position
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(_currentPoint, _rootElement);
            foreach (var element in elements)
            {
                // get automation peer (if already created for this control)
                var automationPeer = FrameworkElementAutomationPeer.FromElement(element) ??
                                     FrameworkElementAutomationPeer.CreatePeerForElement(element);

                //expect null: some elements doesn't have an automation peer implemented
                if (automationPeer == null) continue;

                // horizontal scrolling?
                var ctrlKey = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

                // try to get scroll provider
                // note: TextBoxAutomationPeer does not implement IScrollProvider
                var scrollProvider = automationPeer.GetPattern(PatternInterface.Scroll) as IScrollProvider;
                if (scrollProvider == null) continue;

                // set scoll amount
                var scrollAmount = (delta < 0) ? ScrollAmount.SmallIncrement : ScrollAmount.SmallDecrement;

                // is scrolling horizontal possible
                if (scrollProvider.HorizontallyScrollable && ctrlKey)
                {
                    scrollProvider.Scroll(scrollAmount, ScrollAmount.NoAmount);

                    // break the further serach in the uielement collection
                    break; // foreach
                }
                
                if (scrollProvider.VerticallyScrollable)
                {
                    scrollProvider.Scroll(ScrollAmount.NoAmount, scrollAmount);

                    // break the further serach in the uielement collection
                    break; // foreach
                }

                // don't break here, because of encapsulated scroll viewers such as in the treeview from the sl-toolkit
                //break;
            }
        }
    }
}