using System.Windows.Input;
using EvolutionHighwayApp.Menus.ViewModels;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Menus.Views
{
    public partial class Toolbar
    {
        private ToolbarViewModel ViewModel
        {
            get { return DataContext as ToolbarViewModel; }
        }

        public Toolbar()
        {
            InitializeComponent();

             if (this.InDesignMode()) return;

            DataContext = new ToolbarViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }

        private void ZoomInClick(object sender, System.EventArgs e)
        {
            if (ViewModel.ZoomInCommand.CanExecute(sender))
                ViewModel.ZoomInCommand.Execute(sender);
        }

        private void ZoomOutClick(object sender, System.EventArgs e)
        {
            if (ViewModel.ZoomOutCommand.CanExecute(sender))
                ViewModel.ZoomOutCommand.Execute(sender);
        }

        private void ZoomResetClick(object sender, System.EventArgs e)
        {
            if (ViewModel.ResetZoomCommand.CanExecute(sender))
                ViewModel.ResetZoomCommand.Execute(sender);
        }

        private void GenomeOrientationVerticalClick(object sender, System.EventArgs e)
        {
            if (ViewModel.GenomeOrientationVerticalCommand.CanExecute(sender))
                ViewModel.GenomeOrientationVerticalCommand.Execute(sender);
        }

        private void GenomeOrientationHorizontalClick(object sender, System.EventArgs e)
        {
            if (ViewModel.GenomeOrientationHorizontalCommand.CanExecute(sender))
                ViewModel.GenomeOrientationHorizontalCommand.Execute(sender);
        }

        private void SearchBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ViewModel.SearchCommand.CanExecute(sender))
                ViewModel.SearchCommand.Execute(sender);
        }
    }
}
