using System.Windows.Controls;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;
using Microsoft.Windows;
using DragEventArgs = Microsoft.Windows.DragEventArgs;

namespace EvolutionHighwayApp.Views
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

            DataContext = IoC.Container.Resolve<RefChromosomeSelectorViewModel>();
            Unloaded += delegate { ViewModel.Dispose(); IoC.Container.Release(ViewModel); };
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
