using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Dtos
{
    [DataContract]
    public class SyntenyRegionDto: RegionDto
    {
        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public int? Sign { get; set; }

        [DataMember]
        public double? ModStart { get; set; }

        [DataMember]
        public double? ModEnd { get; set; }

        [DataMember]
        public string Chromosome { get; set; }
    }
}
