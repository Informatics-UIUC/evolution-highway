using System;
using System.Collections.Generic;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.Controllers
{
    public interface IDisplayController
    {
        bool ShowCentromere { get; }
        bool ShowHeterochromatin { get; }
        bool ShowFeatureDensitySparkline { get; }
        bool ShowHighlightRegions { get; }
        bool ShowConservedSynteny { get; }
        bool ShowBreakpointClassification { get; }

        IEnumerable<RefGenome> GetVisibleRefGenomes();

        IEnumerable<RefChromosome> GetVisibleRefChromosomes();
        IEnumerable<RefChromosome> GetVisibleRefChromosomes(RefGenome genome);

        IEnumerable<CompGenome> GetVisibleCompGenomes();
        IEnumerable<CompGenome> GetVisibleCompGenomes(RefChromosome chromosome);

        IEnumerable<Region> GetHighlightRegions(RefChromosome chromosome);

        void UpdateDisplay(ICollection<CompGenome> compGenomes);
        void SetShowCentromere(bool visible, Action continuation = null);
        void SetShowHeterochromatin(bool value, Action continuation = null);
        void SetShowFeatureDensitySparkline(bool visible, Action continuation = null);
        void SetHighlightRegions(RefChromosome chromosome, ICollection<Region> highlightRegions);
        void SetShowConservedSynteny(Action continuation = null);
        void SetShowBreakpointClassification(IEnumerable<string> classNames, double maxThreshold, Action continuation = null);
        void ClearHighlight();
    }
}