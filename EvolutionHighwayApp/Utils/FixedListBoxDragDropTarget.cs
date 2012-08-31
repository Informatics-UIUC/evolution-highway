using System.Windows.Controls;
using System.Windows.Input;

namespace EvolutionHighwayApp.Utils
{
    /// <summary>
    /// Attempting to fix list box drag drop bug reported here: http://silverlight.codeplex.com/workitem/5930
    /// </summary>
    public class FixedListBoxDragDropTarget : ListBoxDragDropTarget
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FixedListBoxDragDropTarget"/> class.
        /// </summary>
        public FixedListBoxDragDropTarget()
        {
            AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(HandleMouseLeftButtonDown), true);
            AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(HandleMouseLeftButtonUp), true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Whether or not the mouse is currently down on the element beneath it.
        /// </summary>
        public static bool IsMouseDown
        {
            get;
            private set;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:MouseButtonEventArgs"/> instance containing the event data.</param>
        private void HandleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:MouseButtonEventArgs"/> instance containing the event data.</param>
        private void HandleMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;
        }

        /// <summary>
        /// Adds all selected items when drag operation begins.
        /// </summary>
        /// <param name="eventArgs">Information about the event.</param>
        protected override void OnItemDragStarting(ItemDragEventArgs eventArgs)
        {
            if (IsMouseDown)
            {
                base.OnItemDragStarting(eventArgs);
            }   // if
            else
            {
                eventArgs.Cancel = true;
                eventArgs.Handled = true;
            }   // else
        }

        #endregion

    }   // class
}
