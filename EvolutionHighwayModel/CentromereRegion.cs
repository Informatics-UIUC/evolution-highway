using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class CentromereRegion : AncestralRegion
    {
        public Chromosome Chromosome { get; set; }
    }
}
