using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Dtos
{
    [DataContract]
    public class RegionDto
    {
        [DataMember]
        public double Start { get; set; }

        [DataMember]
        public double End { get; set; }
    }
}
