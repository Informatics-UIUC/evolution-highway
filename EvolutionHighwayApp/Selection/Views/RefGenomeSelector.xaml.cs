using System.Windows.Controls;
using EvolutionHighwayApp.Selection.ViewModels;
using EvolutionHighwayApp.Utils;
using Microsoft.Windows;

namespace EvolutionHighwayApp.Selection.Views
{
    public partial class RefGenomeSelector
    {
        private bool _itemDropped;

        private RefGenomeSelectorViewModel ViewModel
        {
            get { return DataContext as RefGenomeSelectorViewModel; }
        }

        public RefGenomeSelector()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new RefGenomeSelectorViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            var args = e.Data.GetData(typeof(ItemDragEventArgs)) as ItemDragEventArgs;
            if (args == null) return;

            if (args.DragSource == ((ListBoxDragDropTarget)sender).Content) return;

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            _itemDropped = true;

            #region Unused code

            /*
            var args = e.Data.GetData(typeof(ItemDragEventArgs)) as ItemDragEventArgs;
            if (args == null) return;

            var draggedItems = args.Data as SelectionCollection;
            if (draggedItems == null) return;

            var dropTarget = ((FrameworkElement)e.OriginalSource).DataContext;
            var listBox = (ListBox)((ListBoxDragDropTarget)sender).Content;
            var droppedAtIndex = listBox.Items.IndexOf(dropTarget);

            draggedItems.ForEach(item =>
               Debug.WriteLine("{0} dragged from index {1} to index {2}", ((SelectableItem)item.Item).Name, item.Index, droppedAtIndex++));
            */

            #endregion
        }

        private void OnItemDragCompleted(object sender, ItemDragEventArgs e)
        {
            if (!_itemDropped) return;

            _itemDropped = false;
            ViewModel.OnGenomeSelectionReordered();
        }
    }
}
