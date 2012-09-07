using System.Collections.Generic;

namespace EvolutionHighwayApp.Models
{
    public class RefChromosome : Chromosome
    {
        public Dictionary<string, List<FeatureDensity>> FeatureDensityData { get; private set; }

        public RefChromosome(string name, double length, RefGenome refGenome) : base(name, length, refGenome)
        {
            FeatureDensityData = new Dictionary<string, List<FeatureDensity>>();
        }
    }
}
