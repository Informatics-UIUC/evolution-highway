using System.Runtime.Serialization;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class SpeciesChromosomeLengths
    {
        [DataMember] public string Species { get; set; }
        [DataMember] public string Chromosome { get; set; }
        [DataMember] public double Length { get; set; }
    }
}
