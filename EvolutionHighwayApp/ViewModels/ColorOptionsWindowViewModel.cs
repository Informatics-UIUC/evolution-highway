using System;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.ViewModels
{
    public class ColorOptionsWindowViewModel : ModelBase, IDisposable
    {
        #region ViewModel Bindable Properties

        public AppSettings AppSettings { get; set; }

        #endregion


        public ColorOptionsWindowViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();
        }

        public void Dispose()
        {
        }
    }
}
