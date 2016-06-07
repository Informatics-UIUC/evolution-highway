using System.Collections.Generic;
using System.Windows.Media;
using SilverlightColorChooser;

namespace EvolutionHighwayApp.Utils
{
    public static class PredefinedColorMap
    {
        public static Dictionary<string, Color> Colors = new Dictionary<string, Color>();

        static PredefinedColorMap()
        {
            foreach (var color in PredefinedColor.All)
            {
                Colors.Add(color.Name, color.Value);
            }
        }
    }
}
