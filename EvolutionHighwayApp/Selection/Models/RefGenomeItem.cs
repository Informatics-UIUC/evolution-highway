namespace EvolutionHighwayApp.Selection.Models
{
    public class RefGenomeItem : SelectableItem
    {
        public string Build { get; private set; }
        public string Family { get; private set; }

        public RefGenomeItem(string build, string family)
        {
            Build = build;
            Family = family;
        }
    }
}
