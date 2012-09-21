using System;
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
    }
}
