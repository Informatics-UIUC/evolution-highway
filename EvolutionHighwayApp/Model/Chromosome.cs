using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Model
{
    [DataContract]
    public class Chromosome
    {
        [DataMember] public string Name { get; set; }
        [DataMember] public IEnumerable<ComparativeSpecies> ComparativeSpecies { get; set; }
    }
}
