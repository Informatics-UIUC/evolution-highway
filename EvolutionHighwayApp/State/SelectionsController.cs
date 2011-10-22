using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.State
{
    public class SelectionsController
    {
        public IEnumerable<RefGenome> VisibleRefGenomes { get; private set; }
        public Dictionary<RefGenome, IEnumerable<RefChromosome>> VisibleRefChromosomes { get; private set; }
        public Dictionary<RefChromosome, IEnumerable<CompGenome>> VisibleCompGenomes { get; private set; }

        private readonly IEventPublisher _eventPublisher;
        private readonly Repository _repository;

        public IEnumerable<RefGenome> SelectedRefGenomes { get; set; }
        public IEnumerable<RefChromosome> SelectedRefChromosomes { get; set; }
        public IEnumerable<CompGenome> SelectedCompGenomes { get; private set; } 

        public SelectionsController(IEventPublisher eventPublisher, Repository repository)
        {
            Debug.WriteLine("{0} instantiated", GetType().Name);

            _eventPublisher = eventPublisher;
            _repository = repository;

            InitializeSelections();

            eventPublisher.GetEvent<DataSourceChangedEvent>()
                .Subscribe(e => InitializeSelections());

            eventPublisher.GetEvent<RefGenomeSelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnRefGenomeSelectionChanged);

            eventPublisher.GetEvent<RefChromosomeSelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnRefChromosomeSelectionChanged);

            eventPublisher.GetEvent<CompGenomeSelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnCompGenomeSelectionChanged);
        }

        private void InitializeSelections()
        {
            SelectedRefGenomes = Enumerable.Empty<RefGenome>();
            VisibleRefGenomes = Enumerable.Empty<RefGenome>();

            SelectedRefChromosomes = Enumerable.Empty<RefChromosome>();
            VisibleRefChromosomes = new Dictionary<RefGenome, IEnumerable<RefChromosome>>();

            SelectedCompGenomes = Enumerable.Empty<CompGenome>();
            VisibleCompGenomes = new Dictionary<RefChromosome, IEnumerable<CompGenome>>();
        }

        private void OnRefGenomeSelectionChanged(RefGenomeSelectionChangedEvent e)
        {
            //Debug.WriteLine("Selected genomes: {0}", e.SelectedGenomes.ToString(","));
            SelectedRefGenomes = e.SelectedGenomes;

            if (!e.AddedGenomes.IsEmpty() || !e.RemovedGenomes.IsEmpty() || VisibleRefGenomes.IsEmpty()) 
                return;

            VisibleRefGenomes = e.SelectedGenomes.Intersect(VisibleRefGenomes).ToList();
            _eventPublisher.Publish(new RefGenomeSelectionDisplayEvent
                {
                    AddedGenomes = Enumerable.Empty<RefGenome>(),
                    RemovedGenomes = Enumerable.Empty<RefGenome>(),
                    SelectedGenomes = VisibleRefGenomes
                });
        }

        private void OnRefChromosomeSelectionChanged(RefChromosomeSelectionChangedEvent e)
        {
            //Debug.WriteLine("Selected chromosomes: {0}", e.SelectedChromosomes.ToString(","));
            SelectedRefChromosomes = e.SelectedChromosomes;

            if (!e.AddedChromosomes.IsEmpty() || !e.RemovedChromosomes.IsEmpty()) 
                return;

            e.SelectedChromosomes.Select(c => c.RefGenome).Distinct()
                .Where(g => VisibleRefChromosomes.ContainsKey(g)).ForEach(g =>
                {
                    var selectedRefChromosomes = e.SelectedChromosomes.Where(c => c.RefGenome == g)
                                                    .Intersect(VisibleRefChromosomes[g]).ToList();
                    VisibleRefChromosomes.Set(g, selectedRefChromosomes);
                    _eventPublisher.Publish(new RefChromosomeSelectionDisplayEvent(g)
                        {
                            AddedChromosomes = Enumerable.Empty<RefChromosome>(),
                            RemovedChromosomes = Enumerable.Empty<RefChromosome>(),
                            SelectedChromosomes = selectedRefChromosomes
                        });
                });
        }

        private void OnCompGenomeSelectionChanged(CompGenomeSelectionChangedEvent e)
        {
            //Debug.WriteLine("Selected species: {0}", e.SelectedGenomes.ToString(","));
            SelectedCompGenomes = e.SelectedGenomes;

            if (e.AddedGenomes.IsEmpty() && e.RemovedGenomes.IsEmpty())
            {
                e.SelectedGenomes.Select(g => g.RefChromosome).Distinct().ForEach(c =>
                    {
                        var selectedCompGenomes = e.SelectedGenomes.Where(g => g.RefChromosome == c).ToList();
                        VisibleCompGenomes.Set(c, selectedCompGenomes);
                        _eventPublisher.Publish(new CompGenomeSelectionDisplayEvent(c)
                            {
                                AddedGenomes = Enumerable.Empty<CompGenome>(),
                                RemovedGenomes = Enumerable.Empty<CompGenome>(),
                                SelectedGenomes = selectedCompGenomes
                            });
                    });

                return;
            }

            _repository.LoadSyntenyBlocks(e.AddedGenomes, (result, param) =>
            {
                var displayableRefChromosomes = e.SelectedGenomes.Select(g => g.RefChromosome).Distinct().ToList();
                var displayableRefGenomes = displayableRefChromosomes.Select(c => c.RefGenome).Distinct().ToList();

                var addedRefChromosomes = e.AddedGenomes.Select(g => g.RefChromosome).Distinct().ToList();
                var addedRefGenomes = addedRefChromosomes.Select(c => c.RefGenome)
                                            .Distinct().Except(VisibleRefGenomes).ToList();

                VisibleRefGenomes = SelectedRefGenomes.Intersect(displayableRefGenomes).ToList();

                var removedRefChromosomes = e.RemovedGenomes.Select(g => g.RefChromosome).Distinct().ToList();
                var removedRefGenomes = removedRefChromosomes.Select(c => c.RefGenome)
                                            .Distinct().Except(VisibleRefGenomes).ToList();

                removedRefGenomes.ForEach(g => VisibleRefChromosomes.Remove(g));
                removedRefChromosomes.ForEach(c => VisibleCompGenomes.Remove(c));

                if (!addedRefGenomes.IsEmpty() || !removedRefGenomes.IsEmpty())
                    _eventPublisher.Publish(new RefGenomeSelectionDisplayEvent
                    {
                        AddedGenomes = addedRefGenomes,
                        RemovedGenomes = removedRefGenomes,
                        SelectedGenomes = VisibleRefGenomes
                    });

                displayableRefGenomes.ForEach(g =>
                {
                    var selectedChromosomes = SelectedRefChromosomes.Where(c => c.RefGenome == g).Intersect(
                                                displayableRefChromosomes.Where(c => c.RefGenome == g)).ToList();
                    var addedChromosomes = addedRefChromosomes.Where(c => c.RefGenome == g)
                                                .Except(VisibleRefChromosomes.GetValueOrDefault(g, Enumerable.Empty<RefChromosome>)).ToList();
                    var removedChromosomes = removedRefChromosomes.Where(c => c.RefGenome == g)
                                                .Except(selectedChromosomes).ToList();

                    VisibleRefChromosomes.Set(g, selectedChromosomes);

                    if (!addedChromosomes.IsEmpty() || !removedChromosomes.IsEmpty())
                        _eventPublisher.Publish(new RefChromosomeSelectionDisplayEvent(g)
                        {
                            AddedChromosomes = addedChromosomes,
                            RemovedChromosomes = removedChromosomes,
                            SelectedChromosomes = selectedChromosomes
                        });
                });

                displayableRefChromosomes.Select(c => new CompGenomeSelectionDisplayEvent(c)
                {
                    AddedGenomes = e.AddedGenomes.Where(g => g.RefChromosome == c).ToList(),
                    RemovedGenomes = e.RemovedGenomes.Where(g => g.RefChromosome == c).ToList(),
                    SelectedGenomes = e.SelectedGenomes.Where(g => g.RefChromosome == c).ToList()
                }).ForEach(evt =>
                {
                    VisibleCompGenomes.Set(evt.RefChromosome, evt.SelectedGenomes);
                    if (!evt.AddedGenomes.IsEmpty() || !evt.RemovedGenomes.IsEmpty())
                        _eventPublisher.Publish(evt);
                });
            });
        }
    }
}
