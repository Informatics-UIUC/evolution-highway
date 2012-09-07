using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Repository.Controllers;
using EvolutionHighwayApp.Selection.Controllers;
using EvolutionHighwayApp.Selection.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Selection.ViewModels
{
    public class RefGenomeSelectorViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public SmartObservableCollection<RefGenomeItem> Genomes { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { NotifyPropertyChanged(() => IsLoading, ref _isLoading, value); }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { NotifyPropertyChanged(() => IsEnabled, ref _isEnabled, value); }
        }

        #endregion

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepositoryController _repositoryController;
        private readonly ISelectionController _selectionController;

        private readonly IDisposable _dataSourceChangedObserver;
        private readonly IDisposable _refGenomeSelectionChangedObserver;
        private readonly IDisposable _loadingObserver;
        private readonly IDisposable _refGenomeLoadingObserver;

        public RefGenomeSelectorViewModel()
        {
            IsEnabled = true;

            Genomes = new SmartObservableCollection<RefGenomeItem>();

            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();
            _repositoryController = IoC.Container.Resolve<IRepositoryController>();
            _selectionController = IoC.Container.Resolve<ISelectionController>();

            _dataSourceChangedObserver = _eventPublisher.GetEvent<DataSourceChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnDataSourceChanged);

            _refGenomeSelectionChangedObserver = _eventPublisher.GetEvent<RefGenomeSelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnRefGenomeSelectionChanged);

//            _loadingObserver = EventPublisher.GetEvent<LoadingEvent>()
//                .ObserveOnDispatcher()
//                .Subscribe(e => IsEnabled = e.IsDoneLoading);
//
//            _refGenomeLoadingObserver = EventPublisher.GetEvent<RefGenomesLoadingEvent>()
//                .ObserveOnDispatcher()
//                .Subscribe(e => IsLoading = !e.IsDoneLoading);
        }

        private void OnDataSourceChanged(DataSourceChangedEvent e)
        {
            _repositoryController.GetRefGenomes(
                success =>
                {
                    var items = success.Result.Select(g =>
                    {
                        var item = new RefGenomeItem(g.Name, null);
                        item.PropertyChanged += OnItemPropertyChanged;
                        return item;
                    }).ToList();

                    items.Sort((a, b) => a.Build.CompareTo(b.Build));
                    Genomes.ForEach(item => item.PropertyChanged -= OnItemPropertyChanged);
                    Genomes.ReplaceWith(items);
                });

            // TODO: decide whether to use the beforeLoadCallback in above
        }

        void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine("OnItemPropertyChanged(" + e.PropertyName + ")");
            if (!e.PropertyName.Equals("IsSelected")) return;

            var item = (RefGenomeItem) sender;
            if (item.IsSelected) 
                OnGenomeSelected(item);
            else 
                OnGenomeUnselected(item);
        }

        private void OnGenomeSelected(RefGenomeItem genome)
        {
            _selectionController.SelectRefGenomesByName(Genomes.Where(g => g.IsSelected).Select(g => g.Build).ToList());
        }

        private void OnGenomeUnselected(RefGenomeItem genome)
        {
            _selectionController.SelectRefGenomesByName(Genomes.Where(g => g.IsSelected).Select(g => g.Build).ToList());
        }

        public void OnGenomeSelectionReordered()
        {
            _selectionController.SelectRefGenomesByName(Genomes.Where(g => g.IsSelected).Select(g => g.Build).ToList());
        }

        private void OnRefGenomeSelectionChanged(RefGenomeSelectionChangedEvent e)
        {
            var genomeNames = e.SelectedGenomes.Select(g => g.Name).Distinct().ToList();
            Genomes.ForEach(item => item.PropertyChanged -= OnItemPropertyChanged);
            Genomes.ForEach(g => g.IsSelected = genomeNames.Contains(g.Build));
            Genomes.ForEach(item => item.PropertyChanged += OnItemPropertyChanged);
        }

        public override void Dispose()
        {
            base.Dispose();

            _dataSourceChangedObserver.Dispose();
            _refGenomeSelectionChangedObserver.Dispose();
//            _loadingObserver.Dispose();
//            _refGenomeLoadingObserver.Dispose();

            Genomes.Clear();
        }
    }
}
