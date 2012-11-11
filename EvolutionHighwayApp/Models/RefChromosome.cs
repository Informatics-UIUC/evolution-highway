using System.Collections.Generic;

namespace EvolutionHighwayApp.Models
{
    public class RefChromosome : Chromosome
    {
        public List<FeatureDensity> AdjacencyScore { get; set; }

        public RefChromosome(string name, double length, RefGenome refGenome) : base(name, length, refGenome)
        {
        }
    }
}
