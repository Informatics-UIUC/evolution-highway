using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Dtos
{
    [DataContract]
    public class GenomeDto
    {
        [DataMember]
        public string Name { get; set; }
    }
}
