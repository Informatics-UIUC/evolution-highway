namespace EvolutionHighwayApp.Models
{
    public class CentromereRegion : Region
    {
        public RefChromosome RefChromosome { get; private set; }

        public CentromereRegion(double start, double end, RefChromosome refChromosome) 
            : base(start, end)
        {
            RefChromosome = refChromosome;
        }
    }
}
