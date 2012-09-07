using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.ViewModels
{
    public class CompGenomeNamesViewModel : ViewModelBase
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

        public CompGenomeNamesViewModel()
        {
            IoC.Container.Resolve<IEventPublisher>().GetEvent<CompGenomeNameFormatChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => NotifyPropertyChanged(() => CompGenomes));
        }
    }
}
