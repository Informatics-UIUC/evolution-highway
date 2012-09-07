using System;
using System.Linq;
using System.Windows.Input;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.Commands;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Selection.Controllers;
using EvolutionHighwayApp.Selection.Views;

namespace EvolutionHighwayApp.Selection.ViewModels
{
    public class SelectionsPanelViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public Command ApplySelectionsCommand { get; private set; }

        #endregion

        private readonly ISelectionController _selectionController;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDisposable _dataSourceChangedEventObserver;
        private readonly IDisposable _selectionChangedEventObserver;

        public SelectionsPanelViewModel(SelectionsPanel selectionsPanel)
        {
            _selectionController = IoC.Container.Resolve<ISelectionController>();
            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            _dataSourceChangedEventObserver = _eventPublisher.GetEvent<DataSourceChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => selectionsPanel.ExpandSelections());

            _selectionChangedEventObserver = _eventPublisher.GetEvent<SelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => ApplySelectionsCommand.UpdateCanExecute());

            ApplySelectionsCommand = new Command(ApplySelections, canExecute => _selectionController.IsSelectionPending);
        }

        private void ApplySelections(object param)
        {
            _selectionController.ApplySelections();
            ApplySelectionsCommand.UpdateCanExecute();
        }

        public override void Dispose()
        {
            base.Dispose();

            _dataSourceChangedEventObserver.Dispose();
            _selectionChangedEventObserver.Dispose();
        }
    }
}
