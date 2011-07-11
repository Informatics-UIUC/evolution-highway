using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Dtos
{
    [DataContract]
    public class CompGenomeDto
    {
        [DataMember]
        public string SpeciesName { get; set; }
    }
}
