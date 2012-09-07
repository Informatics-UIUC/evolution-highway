using System.Collections.Generic;

namespace EvolutionHighwayApp.Models
{
    public abstract class Chromosome
    {
        public Genome Genome { get; private set; }

        public string Name { get; private set; }
        public double Length { get; private set; }

        public ICollection<CentromereRegion> CentromereRegions { get; set; }
        public ICollection<HeterochromatinRegion> HeterochromatinRegions { get; set; }

        public ICollection<CompGenome> CompGenomes { get; set; } 

        protected Chromosome(string name, double length, Genome genome)
        {
            Name = name;
            Length = length;
            Genome = genome;

            CentromereRegions = null;
            HeterochromatinRegions = null;
            CompGenomes = null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
