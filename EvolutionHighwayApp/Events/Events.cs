﻿using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Events
{
    #region Menu events

    public class CaptureScreenEvent { }

    public class ShowCentromereEvent 
    {
        public bool ShowCentromere { get; set; }
    }

    public class ShowHeterochromatinEvent
    {
        public bool ShowHeterochromatin { get; set; }
    }

    public class ShowBlockOrientationEvent
    {
        public bool ShowBlockOrientation { get; set; }
    }

    public class ShowAdjacencyScoreEvent
    {
        public bool ShowAdjacencyScore { get; set; }
    }

    public class CompGenomeNameFormatChangedEvent
    {
        public int CompGenomeNameFormat { get; set; }
    }

    #region Layout events

    public abstract class LayoutEvent
    {
        public Orientation Layout { get; set; }
    }

    public class SetGenomeLayoutEvent : LayoutEvent { }
    public class SetChromosomeLayoutEvent : LayoutEvent { }
    public class SetSynBlocksLayoutEvent : LayoutEvent { }

    #endregion

    #endregion

    #region Selection events

    public class DataSourceChangedEvent
    {
        public string DataSourceUrl { get; set; }
    }

    public abstract class GenomeSelectionChangedEvent<T> where T : Genome
    {
        public IEnumerable<T> SelectedGenomes { get; set; }
        public IEnumerable<T> AddedGenomes { get; set; }
        public IEnumerable<T> RemovedGenomes { get; set; }
    }

    public class RefGenomeSelectionChangedEvent : GenomeSelectionChangedEvent<RefGenome> { }
    public class CompGenomeSelectionChangedEvent : GenomeSelectionChangedEvent<CompGenome> { }

    public abstract class ChromosomeSelectionChangedEvent<T> where T : Chromosome
    {
        public IEnumerable<T> SelectedChromosomes { get; set; }
        public IEnumerable<T> AddedChromosomes { get; set; }
        public IEnumerable<T> RemovedChromosomes { get; set; }
    }

    public class RefChromosomeSelectionChangedEvent : ChromosomeSelectionChangedEvent<RefChromosome> { }

    #endregion

    #region Display events

    public abstract class GenomeSelectionDisplayEvent<T> where T : Genome
    {
        public IEnumerable<T> SelectedGenomes { get; set; }
        public IEnumerable<T> AddedGenomes { get; set; }
        public IEnumerable<T> RemovedGenomes { get; set; }
    }

    public class RefGenomeSelectionDisplayEvent : GenomeSelectionDisplayEvent<RefGenome> { }
    public class CompGenomeSelectionDisplayEvent : GenomeSelectionDisplayEvent<CompGenome>
    {
        public RefChromosome RefChromosome { get; private set; }

        public CompGenomeSelectionDisplayEvent(RefChromosome refChromosome)
        {
            RefChromosome = refChromosome;
        }
    }

    public abstract class ChromosomeSelectionDisplayEvent<T> where T : Chromosome
    {
        public IEnumerable<T> SelectedChromosomes { get; set; }
        public IEnumerable<T> AddedChromosomes { get; set; }
        public IEnumerable<T> RemovedChromosomes { get; set; }
    }

    public class RefChromosomeSelectionDisplayEvent : ChromosomeSelectionDisplayEvent<RefChromosome>
    {
        public RefGenome RefGenome { get; private set; }

        public RefChromosomeSelectionDisplayEvent(RefGenome refGenome)
        {
            RefGenome = refGenome;
        }
    }

    public class DisplaySizeChangedEvent
    {
        public double DisplaySize { get; set; }
    }

    public class LabelSizeChangedEvent
    {
        public int LabelSize { get; set; }
    }

    public class BlockWidthChangedEvent
    {
        public double BlockWidth { get; set; }
    }

    public abstract class ColorChangedEvent
    {
        public Color Color { get; set; }        
    }

    public class HeterochromatinBgColorChangedEvent : ColorChangedEvent { }
    public class CentromereBgColorChangedEvent : ColorChangedEvent { }
    public class GenomeInsideBgColorChangedEvent : ColorChangedEvent { }

    #endregion

    #region Loading events

    public abstract class LoadingEvent
    {
        public bool IsDoneLoading { get; set; }
    }

    public class RefGenomesLoadingEvent : LoadingEvent { }
    public class RefChromosomesLoadingEvent : LoadingEvent { }
    public class CompGenomesLoadingEvent : LoadingEvent { }
    public class SyntenyBlocksLoadingEvent : LoadingEvent { }
    public class CentromereRegionsLoadingEvent : LoadingEvent { }
    public class HeterochromatinRegionsLoadingEvent : LoadingEvent { }
    public class CompChrLengthsLoadingEvent : LoadingEvent { }
    public class FeatureLoadingEvent : LoadingEvent
    {
        public string FeatureName { get; set; }
    }

    #endregion
}