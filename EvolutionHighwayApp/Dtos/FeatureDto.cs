using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Dtos
{
    [DataContract]
    public class FeatureDto
    {
        [DataMember]
        public string Name { get; set; }
    }
}
