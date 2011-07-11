namespace EvolutionHighwayApp.Models
{
    public class HeterochromatinRegion : Region
    {
        public RefChromosome RefChromosome { get; private set; }

        public HeterochromatinRegion(double start, double end, RefChromosome refChromosome)
            : base(start, end)
        {
            RefChromosome = refChromosome;
        }
    }
}
