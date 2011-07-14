using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class Menu
    {
        private MenuViewModel ViewModel
        {
            get { return DataContext as MenuViewModel; }
        }

        public Menu()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = IoC.Container.Resolve<MenuViewModel>();
            Unloaded += delegate { ViewModel.Dispose(); IoC.Container.Release(ViewModel); };
        }

        private void OnMenuPopupClosing(object sender, VIBlend.Silverlight.Controls.MenuCancelEventArgs args)
        {
            var focusedElement = FocusManager.GetFocusedElement() as FrameworkElement;

            if (focusedElement is RadioButton || focusedElement is Thumb || focusedElement is ComboBox)
            {
                args.Item.Menu.Focus();
                args.Cancel = true;
            }
        }
    }
}
