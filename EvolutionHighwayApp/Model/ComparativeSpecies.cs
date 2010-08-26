using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Model
{
    [DataContract]
    public class ComparativeSpecies
    {
        [DataMember] public string SpeciesName { get; set; }
        [DataMember] public IEnumerable<AncestorRegion> AncestorRegions { get; set; }
    }
}
