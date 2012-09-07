using System;
using System.Collections.Generic;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Repository.Controllers
{
    public interface IRepositoryController
    {
        //TODO Refactor custom track stuff
        Dictionary<string, SmartObservableCollection<CustomTrackRegion>> CustomTrackMap { get; }
        void AddCustomTrackData(string trackData, char delimiter, bool append = false);
        void ClearCustomTracks();

        /// <summary>
        /// Asynchronously retrieves the collection of known reference genomes
        /// </summary>
        /// <param name="successCallback">The callback to be invoked if the operation is successful</param>
        /// <param name="failureCallback">The callback to be invoked if the operation failed</param>
        /// <param name="beforeLoadCallback">The callback to be invoked before a remote data load (if remote load is required)</param>
        void GetRefGenomes(
            Action<ActionCompletedEventArgs<List<RefGenome>>> successCallback, 
            Action<ActionFailingEventArgs<object>> failureCallback = null,
            Action beforeLoadCallback = null);

        /// <summary>
        /// Asynchronously retrieves the collection of known reference chromosomes for the specified reference genomes
        /// </summary>
        /// <param name="refGenomes">The reference genomes for which to retrieve the chromosomes</param>
        /// <param name="successCallback">The callback to be invoked if the operation is successful</param>
        /// <param name="failureCallback">The callback to be invoked if the operation failed</param>
        /// <param name="beforeLoadCallback">The callback to be invoked before a remote data load (if remote load is required)</param>
        void GetRefChromosomes(ICollection<RefGenome> refGenomes,
            Action<ActionCompletedEventArgs<List<RefChromosome>>> successCallback,
            Action<ActionFailingEventArgs<RefGenome>> failureCallback = null,
            Action beforeLoadCallback = null);

        /// <summary>
        /// Asynchronously retrieves the collection of known comparative genomes for the specified reference chromosomes
        /// </summary>
        /// <param name="refChromosomes">The reference chromosomes for which to retrieve the comparative genomes</param>
        /// <param name="successCallback">The callback to be invoked if the operation is successful</param>
        /// <param name="failureCallback">The callback to be invoked if the operation failed</param>
        /// <param name="beforeLoadCallback">The callback to be invoked before a remote data load (if remote load is required)</param>
        void GetCompGenomes(ICollection<RefChromosome> refChromosomes,
            Action<ActionCompletedEventArgs<List<CompGenome>>> successCallback,
            Action<ActionFailingEventArgs<RefChromosome>> failureCallback = null,
            Action beforeLoadCallback = null);

        /// <summary>
        /// Asynchronously retrieves the synteny blocks for the specified comparative genomes
        /// </summary>
        /// <param name="compGenomes">The comparative genomes for which to retrieve the synteny blocks</param>
        /// <param name="successCallback">The callback to be invoked if the operation is successful</param>
        /// <param name="failureCallback">The callback to be invoked if the operation failed</param>
        /// <param name="beforeLoadCallback">The callback to be invoked before a remote data load (if remote load is required)</param>
        void GetSyntenyBlocks(ICollection<CompGenome> compGenomes,
            Action<ActionCompletedEventArgs<List<SyntenyRegion>>> successCallback,
            Action<ActionFailingEventArgs<CompGenome>> failureCallback = null,
            Action beforeLoadCallback = null);

        void GetCentromereRegions(ICollection<RefChromosome> refChromosomes,
            Action<ActionCompletedEventArgs<List<CentromereRegion>>> successCallback,
            Action<ActionFailingEventArgs<RefChromosome>> failureCallback = null,
            Action beforeLoadCallback = null);

        void GetHeterochromatinRegions(ICollection<RefChromosome> refChromosomes,
            Action<ActionCompletedEventArgs<List<HeterochromatinRegion>>> successCallback,
            Action<ActionFailingEventArgs<RefChromosome>> failureCallback = null,
            Action beforeLoadCallback = null);
    }
}