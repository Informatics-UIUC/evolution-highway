using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Dtos
{
    [DataContract]
    public class ChromosomeDto
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double Length { get; set; }
    }
}
