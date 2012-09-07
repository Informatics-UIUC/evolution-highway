using System.Collections.Generic;

namespace EvolutionHighwayApp.Models
{
    public class RefGenome : Genome
    {
        public ICollection<RefChromosome> RefChromosomes { get; set; }
        
        public RefGenome(string name) : base(name)
        {
            RefChromosomes = null;
        }
    }
}
