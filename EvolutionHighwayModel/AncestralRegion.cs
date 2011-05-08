using System.Runtime.Serialization;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class AncestralRegion
    {
        [DataMember] public double Start { get; set; }
        [DataMember] public double End { get; set; }

        public double Span
        {
            get { return End - Start; }
        }
    }
}
