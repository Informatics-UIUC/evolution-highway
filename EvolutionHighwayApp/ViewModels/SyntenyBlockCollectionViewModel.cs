﻿using EvolutionHighwayApp.Infrastructure;
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

        public AppSettings AppSettings { get; private set; }

        #endregion

        public SyntenyBlockCollectionViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();
        }
    }
}
