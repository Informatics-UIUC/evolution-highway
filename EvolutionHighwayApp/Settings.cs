using System.IO.IsolatedStorage;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp
{
    public class Settings
    {
        private readonly IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;

        public bool ShowCentromere
        {
            get { return (bool) _appSettings.GetValueOrDefault("showCentromere", true); }
            set { _appSettings.Set("showCentromere", value); }
        }

        public bool ShowHeterochromatin
        {
            get { return (bool)_appSettings.GetValueOrDefault("showHeterochromatin", true); }
            set { _appSettings.Set("showHeterochromatin", value); }
        }

        public bool ShowBlockOrientation
        {
            get { return (bool)_appSettings.GetValueOrDefault("showBlockOrientation", false); }
            set { _appSettings.Set("showBlockOrientation", value); }
        }
    }
}
