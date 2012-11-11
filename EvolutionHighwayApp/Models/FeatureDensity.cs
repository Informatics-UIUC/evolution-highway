using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.Models
{
    public class FeatureDensity : ModelBase
    {
        public RefChromosome RefChromosome { get; private set; }

        public double RefStart { get; private set; }
        public double RefEnd { get; private set; }
        public double Score { get; private set; }
        public string CompGen { get; private set; }

        public string ToolTip
        {
            get { return string.Format("Start: {0}\nEnd: {1}\nScore: {2}\nCompGen: {3}", RefStart, RefEnd, Score, CompGen); }
        }

        public FeatureDensity(double refStart, double refEnd, double score, string compGen, RefChromosome refChromosome)
        {
            RefStart = refStart;
            RefEnd = refEnd;
            Score = score;
            CompGen = compGen;
            RefChromosome = refChromosome;
        }
    }
}
