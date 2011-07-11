using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.Models
{
    public abstract class Genome : ModelBase
    {
        public string Name { get; private set; }

        protected Genome(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
