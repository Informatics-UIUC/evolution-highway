using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EvolutionHighwayApp.Settings.Models
{
    public class BreakpointClassificationOptions
    {
        public IEnumerable<string> Classes { get; private set; }

        public double MaxThreshold { get; private set; }

        public BreakpointClassificationOptions(IEnumerable<string> classes, double maxThreshold)
        {
            Classes = classes;
            MaxThreshold = maxThreshold;
        }
    }
}
