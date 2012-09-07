using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.Commands;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Repository.Controllers;
using EvolutionHighwayApp.Selection.Controllers;
using EvolutionHighwayApp.Selection.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Selection.ViewModels
{
    public class CompGenomeSelectorViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public SmartObservableCollection<CompGenomeItem> Genomes { get; private set; }

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

        public ICommand SelectAllCommand { get; private set; }
        #endregion

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepositoryController _repositoryController;
        private readonly ISelectionController _selectionController;

        private readonly IDisposable _dataSourceChangedObserver;
        private readonly IDisposable _refChromosomeSelectionChangedObserver;
        private readonly IDisposable _compGenomeSelectionChangedObserver;
        private readonly IDisposable _loadingObserver;
        private readonly IDisposable _compGenomeLoadingObserver;


        public CompGenomeSelectorViewModel()
        {
            IsEnabled = true;
            Genomes = new SmartObservableCollection<CompGenomeItem>();

            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();
            _repositoryController = IoC.Container.Resolve<IRepositoryController>();
            _selectionController = IoC.Container.Resolve<ISelectionController>();


            _dataSourceChangedObserver = _eventPublisher.GetEvent<DataSourceChangedEvent>()
                .Subscribe(e => { Genomes.ForEach(g => g.PropertyChanged -= OnItemPropertyChanged); Genomes.Clear(); });

            _refChromosomeSelectionChangedObserver = _eventPublisher.GetEvent<RefChromosomeSelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnRefChromosomeSelectionChanged);

            _compGenomeSelectionChangedObserver = _eventPublisher.GetEvent<CompGenomeSelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnCompGenomeSelectionChanged);

//            _loadingObserver = EventPublisher.GetEvent<LoadingEvent>()
//                .ObserveOnDispatcher()
//                .Subscribe(e => IsEnabled = e.IsDoneLoading);
//
//            _compGenomeLoadingObserver = EventPublisher.GetEvent<CompGenomesLoadingEvent>()
//                .ObserveOnDispatcher()
//                .Subscribe(e => IsLoading = !e.IsDoneLoading);

            SelectAllCommand = new Command(SelectAll, canExecute => true);
        }

        private void OnRefChromosomeSelectionChanged(RefChromosomeSelectionChangedEvent e)
        {
            _repositoryController.GetCompGenomes(e.SelectedChromosomes.ToList(),
                success =>
                {
                    var previouslySelectedCompGenNames = Genomes.Where(g => g.IsSelected).Select(c => c.Name).ToList();
                    var compGenNames = success.Result.Select(g => g.Name).Distinct().ToList();
                    compGenNames.Sort();
                    var items = compGenNames.Select(
                        name =>
                        {
                            var item = new CompGenomeItem(name);
                            if (previouslySelectedCompGenNames.Contains(name))
                                item.IsSelected = true;
                            item.PropertyChanged += OnItemPropertyChanged;
                            return item;
                        });

                    Genomes.ForEach(g => g.PropertyChanged -= OnItemPropertyChanged);
                    Genomes.ReplaceWith(items);
                });

            // TODO: decide whether to use the beforeLoadCallback in above
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("IsSelected")) return;

            var item = (CompGenomeItem)sender;
            if (item.IsSelected)
                OnGenomeSelected(item);
            else
                OnGenomeUnselected(item);
        }

        private void OnGenomeSelected(CompGenomeItem genome)
        {
            _selectionController.SelectCompGenomesByName(Genomes.Where(g => g.IsSelected).Select(g => g.Name).ToList());
        }

        private void OnGenomeUnselected(CompGenomeItem genome)
        {
            _selectionController.SelectCompGenomesByName(Genomes.Where(g => g.IsSelected).Select(g => g.Name).ToList());
        }

        public void OnGenomeSelectionReordered()
        {
            _selectionController.SelectCompGenomesByName(Genomes.Where(g => g.IsSelected).Select(g => g.Name).ToList());
        }

        private void SelectAll(object param)
        {
            var selectAll = bool.Parse(param.ToString());

            var selected = selectAll ? Genomes.Select(g => g.Name) : Enumerable.Empty<string>();
            _selectionController.SelectCompGenomesByName(selected.ToArray());
        }

        private void OnCompGenomeSelectionChanged(CompGenomeSelectionChangedEvent e)
        {
            var selectedGenomeNames = e.SelectedGenomes.Select(c => c.Name).Distinct().ToList();
            Genomes.ForEach(item =>
                                {
                                    item.PropertyChanged -= OnItemPropertyChanged;
                                    item.IsSelected = selectedGenomeNames.Contains(item.Name);
                                    item.PropertyChanged += OnItemPropertyChanged;
                                });
        }

        public override void Dispose()
        {
            base.Dispose();

            _dataSourceChangedObserver.Dispose();
            _refChromosomeSelectionChangedObserver.Dispose();
            _compGenomeSelectionChangedObserver.Dispose();
//            _loadingObserver.Dispose();
//            _compGenomeLoadingObserver.Dispose();

            Genomes.Clear();
        }
    }
}
