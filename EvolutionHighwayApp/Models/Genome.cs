namespace EvolutionHighwayApp.Models
{
    //TODO Implement GetHashcode and override equals since we're using these as keys into dictionaries
    public abstract class Genome
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
