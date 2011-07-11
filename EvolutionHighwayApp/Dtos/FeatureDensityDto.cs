using System;
using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Dtos
{
    [DataContract]
    public class FeatureDensityDto
    {
        [DataMember]
        public double RefStart { get; set; }

        [DataMember]
        public double RefEnd { get; set; }

        [DataMember]
        public double Score { get; set; }

        [DataMember]
        public String CompGen { get; set; }
    }
}
