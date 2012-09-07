using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EvolutionHighwayApp.Settings.Models;
using EvolutionHighwayApp.Settings.ViewModels;

namespace EvolutionHighwayApp.Settings.Views
{
    public partial class BreakpointClassificationOptionsDialog
    {
        private BreakpointClassificationOptionsDialogViewModel ViewModel
        {
            get { return DataContext as BreakpointClassificationOptionsDialogViewModel; }
        }

        public BreakpointClassificationOptions Options { get; private set; }

        private bool _maxThresholdValid = true;

        public BreakpointClassificationOptionsDialog(BreakpointClassificationOptions options)
        {
            InitializeComponent();

            Options = options;

            if (options.Classes.IsEmpty())
                _btnOk.IsEnabled = false;

            DataContext = new BreakpointClassificationOptionsDialogViewModel(options);
            Loaded += delegate { if (!options.Classes.IsEmpty()) _lstClasses.SelectedIndex = 0; };
            Unloaded += delegate { ViewModel.Dispose(); };
        }

        private void OnOKButtonClick(object sender, RoutedEventArgs e)
        {
            Options = new BreakpointClassificationOptions(
                _lstClasses.SelectedItems.Cast<string>().ToList(),
                ViewModel.MaxThreshold * 1e6);

            DialogResult = true;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnBindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            switch (e.Action)
            {
                case ValidationErrorEventAction.Added:
                    _btnOk.IsEnabled = false;
                    _maxThresholdValid = false;
                    break;

                case ValidationErrorEventAction.Removed:
                    _maxThresholdValid = true;
                    _btnOk.IsEnabled = _lstClasses.SelectedItems.Count > 0;
                    break;
            }
        }

        private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _btnOk.IsEnabled = _maxThresholdValid && (_lstClasses.SelectedItems.Count > 0);
        }
    }
}

