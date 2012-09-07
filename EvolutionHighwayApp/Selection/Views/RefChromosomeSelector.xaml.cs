using System.Windows.Controls;
using EvolutionHighwayApp.Selection.ViewModels;
using EvolutionHighwayApp.Utils;
using Microsoft.Windows;

namespace EvolutionHighwayApp.Selection.Views
{
    public partial class RefChromosomeSelector
    {
        private bool _itemDropped;

        private RefChromosomeSelectorViewModel ViewModel
        {
            get { return DataContext as RefChromosomeSelectorViewModel; }
        }

        public RefChromosomeSelector()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new RefChromosomeSelectorViewModel();
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
        }

        private void OnItemDragCompleted(object sender, ItemDragEventArgs e)
        {
            if (!_itemDropped) return;

            _itemDropped = false;
            ViewModel.OnChromosomeSelectionReordered();
        }
    }
}
