using System;
using System.Linq;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.ViewModels
{
    public class SyntenyBlockViewModel : ModelBase, IDisposable
    {
        #region ViewModel Bindable Properties

        public AppSettings AppSettings { get; private set; }

        private SyntenyRegion _syntenyRegion;
        public SyntenyRegion SyntenyRegion
        {
            get { return _syntenyRegion; }
            set
            {
                NotifyPropertyChanged(() => SyntenyRegion, ref _syntenyRegion, value);
                BlockOrientation = new BlockOrientation(value, AppSettings.BlockWidth);
            }
        }

        private BlockOrientation _blockOrientation;
        public BlockOrientation BlockOrientation
        {
            get { return _blockOrientation; }
            private set { NotifyPropertyChanged(() => BlockOrientation, ref _blockOrientation, value); }
        }

        #endregion

        private readonly IDisposable _displaySizeChangedObserver;
        private readonly IDisposable _blockWidthChangedObserver;


        public SyntenyBlockViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();

            var eventPublisher = IoC.Container.Resolve<IEventPublisher>();
            _displaySizeChangedObserver = eventPublisher.GetEvent<DisplaySizeChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => NotifyPropertyChanged(() => SyntenyRegion));

            _blockWidthChangedObserver = eventPublisher.GetEvent<BlockWidthChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e =>
                               {
                                   BlockOrientation.BlockWidth = e.BlockWidth;
                                   NotifyPropertyChanged(() => BlockOrientation);
                                });
        }

        public void Dispose()
        {
            _displaySizeChangedObserver.Dispose();
            _blockWidthChangedObserver.Dispose();
        }
    }
}
