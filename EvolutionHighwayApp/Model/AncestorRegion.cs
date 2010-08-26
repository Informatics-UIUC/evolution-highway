using System.Runtime.Serialization;

namespace EvolutionHighwayApp.Model
{
    [DataContract]
    public class AncestorRegion
    {
        [DataMember] public double Start { get; set; }
        [DataMember] public double End { get; set; }
        [DataMember] public string Label { get; set; }

        public double Span
        {
            get { return End - Start; }
        }
    }
}
