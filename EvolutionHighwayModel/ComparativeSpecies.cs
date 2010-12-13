using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class ComparativeSpecies
    {
        [DataMember] public string SpeciesName { get; set; }
        [DataMember] public IEnumerable<AncestorRegion> AncestorRegions { get; set; }

        public static IDictionary<Tuple<string, string>, double> ChromosomeLengths =
            new Dictionary<Tuple<string, string>, double>();

        public Chromosome Chromosome { get; set; }
    }
}
