using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class Genome
    {
        [DataMember] public string Name { get; set; }
        [DataMember] public IEnumerable<Chromosome> Chromosomes { get; set; }
    }
}
