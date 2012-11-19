using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Utils
{
    public static class DataExport
    {
        public static string ConservedSyntenyToCSV(RefChromosome refChr, IEnumerable<ConservedSyntenyHighlightRegion> conservedRegions)
        {
            var sb = new StringBuilder();
            conservedRegions.ForEach(
                cr => sb.AppendFormat("{0},{1},{2},{3},{4}{5}",
                    Csv.Escape(refChr.Genome.Name), Csv.Escape(refChr.Name), cr.Start, cr.End, cr.Span, Environment.NewLine));

            return sb.ToString();
        }

        public static string BreakpointClassesToCSV(RefChromosome refChr, IEnumerable<BreakpointClassificationHighlightRegion> breakpointRegions)
        {
            var sb = new StringBuilder();
            breakpointRegions.ForEach(
                cr => sb.AppendFormat("{0},{1},{2},{3},{4}{5}",
                    Csv.Escape(refChr.Genome.Name), Csv.Escape(refChr.Name), cr.Start, cr.End, cr.Span, Environment.NewLine));

            return sb.ToString();
        }

        public static string BreakpointScoreToCSV(IEnumerable<BreakpointRegion> breakpointRegions)
        {
            var sb = new StringBuilder();
            int n = breakpointRegions.Select(br => br.CompGenome.Name).Distinct().Count();
            breakpointRegions.ForEach(
                br => sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6}{7}",
                    Csv.Escape(br.CompGenome.RefChromosome.Genome.Name),
                    Csv.Escape(br.CompGenome.RefChromosome.Name),
                    Csv.Escape(br.CompGenome.Name),
                    br.Start, br.End, Csv.Escape(br.Type.ToString()), br.GetScore(1f/(n-1)), Environment.NewLine));

            return sb.ToString();
        }
    }
}
