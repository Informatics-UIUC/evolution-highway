using System.Runtime.Serialization;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class AncestorRegion
    {
        [DataMember] public double Start { get; set; }
        [DataMember] public double End { get; set; }
        [DataMember] public string Label { get; set; }
        [DataMember] public int? Sign { get; set; }

        public ComparativeSpecies ComparativeSpecies { get; set; }

        public double Span
        {
            get { return End - Start; }
        }
    }
}
