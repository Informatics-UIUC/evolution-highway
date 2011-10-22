﻿using System.Collections.Generic;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Models
{
    public class RefChromosome : Chromosome
    {
        public double Length { get; private set; }
        public RefGenome RefGenome { get; private set; }

        public List<CentromereRegion> CentromereRegions { get; private set; }
        public List<HeterochromatinRegion> HeterochromatinRegions { get; private set; }

        public SmartObservableCollection<Region> HighlightRegions { get; private set; } 

        public Dictionary<string, List<FeatureDensity>> FeatureDensityData { get; private set; }

        public RefChromosome(string name, double length, RefGenome refGenome) : base(name)
        {
            Length = length;
            RefGenome = refGenome;
            CentromereRegions = new List<CentromereRegion>();
            HeterochromatinRegions = new List<HeterochromatinRegion>();
            FeatureDensityData = new Dictionary<string, List<FeatureDensity>>();
            HighlightRegions = new SmartObservableCollection<Region>();
        }
    }
}
