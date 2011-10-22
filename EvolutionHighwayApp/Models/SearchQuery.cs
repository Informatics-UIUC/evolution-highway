namespace EvolutionHighwayApp.Models
{
    public class SearchQuery
    {
        public string RefGenomeName { get; set; }
        public string RefChromosomeName { get; set; }
        public string CompGenomeName { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
    }
}
