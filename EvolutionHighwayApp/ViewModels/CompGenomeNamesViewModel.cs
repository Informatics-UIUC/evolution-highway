using System.Collections.Generic;
using System.Windows;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.ViewModels
{
    public class CompGenomeNamesViewModel : ModelBase
    {
        #region ViewModel Bindable Properties

        private IEnumerable<CompGenome> _compGenomes;
        public IEnumerable<CompGenome> CompGenomes
        {
            get { return _compGenomes; }
            set { NotifyPropertyChanged(() => CompGenomes, ref _compGenomes, value); }
        }

        private int _namesWidth;
        public int NamesWidth
        {
            get { return _namesWidth; }
            set { NotifyPropertyChanged(() => NamesWidth, ref _namesWidth, value); }
        }

        private VerticalAlignment _namesAlignment;
        public VerticalAlignment NamesAlignment
        {
            get { return _namesAlignment; }
            set { NotifyPropertyChanged(() => NamesAlignment, ref _namesAlignment, value); }
        }

        #endregion
    }
}
