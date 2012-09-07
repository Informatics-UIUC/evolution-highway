namespace EvolutionHighwayApp.Selection.Models
{
    public class CompGenomeItem : SelectableItem
    {
        public string Name { get; private set; }

        public CompGenomeItem(string name)
        {
            Name = name;
        }
    }
}
