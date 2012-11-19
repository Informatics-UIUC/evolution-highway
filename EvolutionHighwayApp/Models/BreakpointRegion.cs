using System;
using System.Collections.Generic;
using System.Linq;

namespace EvolutionHighwayApp.Models
{
    public enum BreakpointRegionType
    {
        Breakpoint,
        Gap
    };

    public class BreakpointRegion : Region
    {
        public CompGenome CompGenome { get; private set; }
        public BreakpointRegionType Type
        {
            get
            {
                var breakpointsByCompGen = _breakpointOverlaps.GroupBy(br => br.CompGenome).ToDictionary(elk => elk.Key, elv => elv.ToList());

                // Rule 1 - the breakpoint must not overlap more than 1 breakpoint for each compGen
                if (breakpointsByCompGen.Any(br => br.Value.Count > 1)) 
                    return BreakpointRegionType.Gap;

                // Rule 2 - all breakpoint overlaps for the other compGens must themselves pairwise overlap
                if (_breakpointOverlaps.Any(br1 =>
                    _breakpointOverlaps.Where(br2 => 
                        String.Compare(br1.CompGenome.Name, br2.CompGenome.Name, StringComparison.Ordinal) < 0)
                        .Any(br2 => !br1.Overlaps(br2)))) 
                    return BreakpointRegionType.Gap;

                return BreakpointRegionType.Breakpoint;
            }
        }

        public int OverlappingGapCount
        {
            get { return _breakpointOverlaps.Count(br => br.Type == BreakpointRegionType.Gap); }
        }

        public int OverlappingBreakpointCount
        {
            get { return _breakpointOverlaps.Count(br => br.Type == BreakpointRegionType.Breakpoint); }
        }

        public int OverlapCount
        {
            get { return _breakpointOverlaps.Count; }
        }

        public int NonOverlapCount
        {
            get { return -1; }
        }

        private readonly List<BreakpointRegion> _breakpointOverlaps;

        public BreakpointRegion(double start, double end, CompGenome compGenome) : base(start, end)
        {
            CompGenome = compGenome;

            _breakpointOverlaps = new List<BreakpointRegion>();
        }

        public bool Overlaps(BreakpointRegion other)
        {
            var start = Math.Max(Start, other.Start);
            var end = Math.Min(End, other.End);

            return start < end;
        }

        public void AddOverlap(BreakpointRegion other)
        {
            _breakpointOverlaps.Add(other);
        }

        public float GetScore(float minScore)
        {
            switch (Type)
            {
                case BreakpointRegionType.Breakpoint:
                    return (OverlappingBreakpointCount + OverlappingGapCount/2)*minScore;

                case BreakpointRegionType.Gap:
                    return -1; // TODO FIXME

                default:
                    return -1;
            }
        }

        public override string ToString()
        {
            return string.Format("chr: {0} compGen: {1} [start: {2} end: {3} type: {4}]",
                                 CompGenome.RefChromosome.Name, CompGenome.Name, Start, End, Type);
        }
    }
}
