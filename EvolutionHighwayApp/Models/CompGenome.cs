using System.Collections.Generic;

namespace EvolutionHighwayApp.Models
{
    public class CompGenome : Genome
    {
        public RefChromosome RefChromosome { get; private set; }

        public ICollection<SyntenyRegion> SyntenyBlocks { get; set; }

        public CompGenome(string name, RefChromosome refChromosome) : base(name)
        {
            RefChromosome = refChromosome;
            SyntenyBlocks = null;
        }
    }
}
