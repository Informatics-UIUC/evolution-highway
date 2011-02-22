using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class Chromosome
    {
        [DataMember] public string Name { get; set; }
        [DataMember] public IEnumerable<ComparativeSpecies> ComparativeSpecies { get; set; }

        public Genome Genome { get; set; }

        public double Length { get; set; }
    }
}
