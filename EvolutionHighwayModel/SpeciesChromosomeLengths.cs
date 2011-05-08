using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class SpeciesChromosomeLengths
    {
        [DataMember] public string Chromosome { get; set; }
        [DataMember] public double Length { get; set; }

        public static readonly IDictionary<string, IDictionary<string, double>> ChromosomeLengths 
            = new Dictionary<string, IDictionary<string, double>>();
    }
}
