using System.Collections.Generic;
using System.Linq;
using EvolutionHighwayApp.Exceptions;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using SilverlightColorChooser;

namespace EvolutionHighwayApp.Repository.Models
{
    // TODO: Idea: have the ViewModels register with the state such that the state has a reference to all the viewmodels for regen, refchr, compgen...
    // You can then manipulate the viewmodels directly instead of relying on events... may be faster this way
    public class RepositoryState
    {
        public List<RefGenome> RefGenomes { get; set; }
        public Dictionary<string, SmartObservableCollection<CustomTrackRegion>> CustomTrackMap { get; private set; }

        public static IDictionary<string, IDictionary<string, double>> CompChromosomeLengths;

        static RepositoryState()
        {
            CompChromosomeLengths = new Dictionary<string, IDictionary<string, double>>();
        }

        public RepositoryState()
        {
            RefGenomes = null;
            CustomTrackMap = new Dictionary<string, SmartObservableCollection<CustomTrackRegion>>();
        }

        public void AddCustomTrackData(string trackData, char delimiter, bool append = false)
        {
            var lines = trackData.Split(new[] { '\n', '\r' });
            var lineno = 0;

            if (!append)
                // Clear any existing custom tracks
                ClearCustomTracks();

            foreach (var line in lines.Where(line => !string.IsNullOrWhiteSpace(line)))
            {
                lineno++;

                var parts = line.Split(delimiter);

                if (parts.Length != 5 && parts.Length != 6)
                    throw new ParseErrorException(line, lineno);

                var genome = parts[0];
                var chromosome = parts[1];
                var label = parts[2];
                var start = double.Parse(parts[3]);
                var end = double.Parse(parts[4]);


                var color = PredefinedColor.AllColors["Black"];
                if (parts.Length == 6)
                {
                    var colorStr = parts[5];
                    if (colorStr.StartsWith("#"))
                        color = color.FromHexString(colorStr);
                    else
                        if (PredefinedColor.AllColors.ContainsKey(colorStr))
                            color = PredefinedColor.AllColors[colorStr];
                        else
                            throw new ParseErrorException(colorStr, lineno);
                }

                var key = string.Format("<{0}><{1}>", genome, chromosome);

                var trackRegion = new CustomTrackRegion(genome, chromosome, start, end, label, color);

                if (!CustomTrackMap.ContainsKey(key))
                    CustomTrackMap.Add(key, new SmartObservableCollection<CustomTrackRegion>());

                var trackRegions = CustomTrackMap[key];
                trackRegions.Add(trackRegion);
            }
        }

        public void ClearCustomTracks()
        {
            CustomTrackMap.ForEach(kvp => kvp.Value.Clear());
            CustomTrackMap.Clear();
        }

        public void Clear()
        {
            RefGenomes = null;
            CustomTrackMap.Clear();
            CompChromosomeLengths = new Dictionary<string, IDictionary<string, double>>();
        }
    }
}
