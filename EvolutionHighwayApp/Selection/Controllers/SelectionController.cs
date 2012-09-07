using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EvolutionHighwayApp.Display.Controllers;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Repository.Controllers;

namespace EvolutionHighwayApp.Selection.Controllers
{
    public class SelectionController : ISelectionController
    {
        public ICollection<RefGenome> SelectedRefGenomes
        {
            get { return _selectedRefGenomes ?? new RefGenome[0]; }
        }

        public ICollection<RefChromosome> SelectedRefChromosomes
        {
            get { return _selectedRefChromosomes ?? new RefChromosome[0]; }
        }

        public ICollection<CompGenome> SelectedCompGenomes
        {
            get { return _selectedCompGenomes ?? new CompGenome[0]; }
        }

        public bool IsSelectionPending
        {
            get
            {
                return 
                    (_pendingRefGenomeSelection != null && !_pendingRefGenomeSelection.SequenceEqual(SelectedRefGenomes)) || 
                    (_pendingRefChromosomeSelection != null && !_pendingRefChromosomeSelection.SequenceEqual(SelectedRefChromosomes)) ||
                    (_pendingCompGenomeSelection != null && !_pendingCompGenomeSelection.SequenceEqual(SelectedCompGenomes));
            }
        }

        private readonly IRepositoryController _repositoryController;
        private readonly IDisplayController _displayController;
        private readonly IEventPublisher _eventPublisher;

        private ICollection<RefGenome> _selectedRefGenomes; 
        private ICollection<RefGenome> _pendingRefGenomeSelection;

        private ICollection<RefChromosome> _selectedRefChromosomes; 
        private ICollection<RefChromosome> _pendingRefChromosomeSelection;

        private ICollection<CompGenome> _selectedCompGenomes; 
        private ICollection<CompGenome> _pendingCompGenomeSelection;


        public SelectionController(IRepositoryController repositoryController, IDisplayController displayController, IEventPublisher eventPublisher)
        {
            _repositoryController = repositoryController;
            _displayController = displayController;
            _eventPublisher = eventPublisher;

            _eventPublisher.GetEvent<DataSourceChangedEvent>()
                .Subscribe(e => InitializeSelections());

            InitializeSelections();
        }

        private void InitializeSelections()
        {
            _selectedRefGenomes = null;
            _selectedRefChromosomes = null;
            _selectedCompGenomes = null;

            ResetPendingSelections();
        }

        private void ResetPendingSelections()
        {
            _pendingRefGenomeSelection = null;
            _pendingRefChromosomeSelection = null;
            _pendingCompGenomeSelection = null;
        }

        public void SelectRefGenomesByName(ICollection<string> refGenomeNames, Action<IEnumerable<RefGenome>> continuation = null)
        {
            _repositoryController.GetRefGenomes(
                success =>
                {
                    var refGenomes = success.Result;
                    _pendingRefGenomeSelection = refGenomeNames.SelectMany(name => refGenomes.Where(g => g.Name == name)).ToList();

                    // TODO: Think about why the below is needed --- maybe you can do it different to not trigger cascaded selects
                    var refChromosomeNames = (_pendingRefChromosomeSelection ?? SelectedRefChromosomes)
                        .Select(c => c.Name).Distinct().ToList();

                    SelectRefChromosomesByName(refChromosomeNames,
                        c =>
                        {
                            _eventPublisher.Publish(new RefGenomeSelectionChangedEvent { SelectedGenomes = _pendingRefGenomeSelection });

                            if (continuation != null) 
                                continuation(_pendingRefGenomeSelection.ToList());
                        });
                });
        }

        public void SelectRefChromosomesByName(ICollection<string> refChromosomeNames, Action<IEnumerable<RefChromosome>> continuation = null)
        {
            var refGenomes = (_pendingRefGenomeSelection ?? SelectedRefGenomes).ToList();

            _repositoryController.GetRefChromosomes(refGenomes,
                success =>
                {
                    var refChromosomes = success.Result;
                    _pendingRefChromosomeSelection = refChromosomes.GroupBy(c => c.Genome)
                        .SelectMany(chromosomes => refChromosomeNames.SelectMany(
                            name => chromosomes.Where(c => c.Name == name))).ToList();
    
                    var compGenomeNames = (_pendingCompGenomeSelection ?? SelectedCompGenomes)
                        .Select(g => g.Name).Distinct().ToList();

                    SelectCompGenomesByName(compGenomeNames,
                        c =>
                        {
                            _eventPublisher.Publish(new RefChromosomeSelectionChangedEvent { SelectedChromosomes = _pendingRefChromosomeSelection });

                            if (continuation != null)
                                continuation(_pendingRefChromosomeSelection.ToList());
                        });
                });
        }

        public void SelectCompGenomesByName(ICollection<string> compGenomeNames, Action<IEnumerable<CompGenome>> continuation = null)
        {
            var refChromosomes = (_pendingRefChromosomeSelection ?? SelectedRefChromosomes).ToList();

            _repositoryController.GetCompGenomes(refChromosomes,
                success =>
                {
                    var compGenomes = success.Result;
                    _pendingCompGenomeSelection = compGenomes.GroupBy(g => g.RefChromosome)
                        .SelectMany(genomes => compGenomeNames.SelectMany(
                            name => genomes.Where(g => g.Name == name))).ToList();

                    _eventPublisher.Publish(new CompGenomeSelectionChangedEvent { SelectedGenomes = _pendingCompGenomeSelection });

                    if (continuation != null) 
                        continuation(_pendingCompGenomeSelection.ToList());
                });
        }

        public void ApplySelections()
        {
            Debug.WriteLine("ApplySelections()");

            if (_pendingRefGenomeSelection != null)
                _selectedRefGenomes = _pendingRefGenomeSelection;

            if (_pendingRefChromosomeSelection != null)
                _selectedRefChromosomes = _pendingRefChromosomeSelection;

            if (_pendingCompGenomeSelection != null)
                _selectedCompGenomes = _pendingCompGenomeSelection;

            ResetPendingSelections();

            _displayController.UpdateDisplay(_selectedCompGenomes);
        }
    }
}
