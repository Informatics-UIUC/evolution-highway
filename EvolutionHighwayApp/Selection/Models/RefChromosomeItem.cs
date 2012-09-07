namespace EvolutionHighwayApp.Selection.Models
{
    public class RefChromosomeItem : SelectableItem
    {
        public string Name { get; private set; }

        public RefChromosomeItem(string name)
        {
            Name = name;
        }
    }
}
