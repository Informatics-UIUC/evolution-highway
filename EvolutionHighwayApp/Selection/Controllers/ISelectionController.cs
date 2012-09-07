using System;
using System.Collections.Generic;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Selection.Controllers
{
    /// <summary>
    /// Interface providing methods to control the selection of reference genomes, reference chromosomes, and comparative genomes
    /// </summary>
    public interface ISelectionController
    {
        /// <summary>
        /// The list of currently selected reference genomes
        /// </summary>
        ICollection<RefGenome> SelectedRefGenomes { get; }

        /// <summary>
        /// The list of currently selected reference chromosomes
        /// </summary>
        ICollection<RefChromosome> SelectedRefChromosomes { get; }

        /// <summary>
        /// The list of currently selected comparative genomes
        /// </summary>
        ICollection<CompGenome> SelectedCompGenomes { get; }

        bool IsSelectionPending { get; }

        /// <summary>
        /// Add to selection all reference genomes with the given names
        /// </summary>
        /// <param name="refGenomeNames">The reference genome names to add to the selection</param>
        /// <param name="continuation">(Optional) The continuation method to be called after selection</param>
        void SelectRefGenomesByName(ICollection<string> refGenomeNames, Action<IEnumerable<RefGenome>>  continuation = null);

        /// <summary>
        /// Add to selection all reference chromosomes with the given names
        /// </summary>
        /// <param name="refChromosomeNames">The reference chromosome names to add to the selection</param>
        /// <param name="continuation">(Optional) The continuation method to be called after selection</param>
        void SelectRefChromosomesByName(ICollection<string> refChromosomeNames, Action<IEnumerable<RefChromosome>>  continuation = null);

        /// <summary>
        /// Add to selection all comparative genomes with the given names
        /// </summary>
        /// <param name="compGenomeNames">The comparative genome names to add to the selection</param>
        /// <param name="continuation">(Optional) The continuation method to be called after selection</param>
        void SelectCompGenomesByName(ICollection<string> compGenomeNames, Action<IEnumerable<CompGenome>>  continuation = null);

        /// <summary>
        /// Triggers the SelectionsUpdated event specifying the changes made to the selections since the last ApplySelections was called
        /// </summary>
        void ApplySelections();
    }
}