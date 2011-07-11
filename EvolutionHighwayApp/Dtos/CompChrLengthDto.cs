using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Dtos
{
    [DataContract]
    public class CompChrLengthDto
    {
        [DataMember]
        public string Chromosome { get; set; }

        [DataMember]
        public double Length { get; set; }
    }
}
