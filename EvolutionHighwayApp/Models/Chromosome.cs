using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.Models
{
    public abstract class Chromosome : ModelBase
    {
        public string Name { get; private set; }

        protected Chromosome(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
