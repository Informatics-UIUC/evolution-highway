using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.Settings.ViewModels
{
    public class ColorOptionsWindowViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public AppSettings AppSettings { get; set; }

        #endregion

        public ColorOptionsWindowViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();
        }
    }
}
