using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.ViewModels
{
    public class HighlightRegionCollectionViewModel : ModelBase
    {
        #region ViewModel Bindable Properties

        private RefChromosome _refChromosome;
        public RefChromosome RefChromosome
        {
            get { return _refChromosome; }
            set
            {
                if (RefChromosome == value) return;
                _refChromosome = value;

                NotifyPropertyChanged(() => RefChromosome);
            }
        }

        #endregion
    }
}
