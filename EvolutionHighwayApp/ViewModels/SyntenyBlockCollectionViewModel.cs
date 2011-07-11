using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.ViewModels
{
    public class SyntenyBlockCollectionViewModel : ModelBase
    {
        #region ViewModel Bindable Properties

        private CompGenome _compGenome;
        public CompGenome CompGenome
        {
            get { return _compGenome; }
            set { NotifyPropertyChanged(() => CompGenome, ref _compGenome, value); }
        }

        #endregion
    }
}
