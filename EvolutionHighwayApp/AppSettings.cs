using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using System.Windows.Media;
using EvolutionHighwayApp.Display.Controllers;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp
{
    public class AppSettings : ViewModelBase
    {
        private readonly IDisplayController _displayController;
        private readonly IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;
        private readonly Dictionary<string, object> _defaultValues;

        public IEventPublisher EventPublisher { get; private set; }

        public static int DisplaySizeMinimum { get { return 50; } }
        public static int DisplaySizeMaximum { get { return 20000; } }
        public static int DisplaySizeSmallChange { get { return 50; } }
        public static int DisplaySizeLargeChange { get { return 2000; } }


        public AppSettings(IDisplayController displayController)
        {
            _displayController = displayController;

            EventPublisher = IoC.Container.Resolve<IEventPublisher>();

            _defaultValues = new Dictionary<string, object>
                                 {
                                     {"showCentromere", true},
                                     {"showHeterochromatin", true},
                                     {"showBlockOrientation", false},
                                     {"showAdjacencyScore", false},
                                     {"showScale", true },
                                     {"genomeLayout", Orientation.Horizontal},
                                     {"chromosomeLayout", Orientation.Horizontal},
                                     {"synBlocksLayout", Orientation.Vertical},
                                     {"displaySize", 500d},
                                     {"blockWidth", 24d},
                                     {"compGenomeNameFormat", 7},
                                     {"labelSize", 55},
                                     {"centromereBgColor", Colors.Black},
                                     {"heterochromatinBgColor", Colors.Transparent},
                                     {"genomeInsideBgColor", Colors.White},
                                     {"featureDensityBgColor", PredefinedColorMap.Colors["Alice Blue"]},
                                     {"featureDensityFillColor", Colors.Transparent},
                                     {"sparklineColor", Colors.Black},
                                     {"searchHighlightColor", PredefinedColorMap.Colors["Light Green"]},
                                     {"conservedSyntenyHighlightColor", PredefinedColorMap.Colors["Pale Green"]},
                                     {"breakpointClassificationHighlightColor", PredefinedColorMap.Colors["Plum"]},
                                     {"breakpointClassificationMaxThreshold", 100000000d},
                                     {"dataPointFillColor", PredefinedColorMap.Colors["Light Salmon"]},
                                     {"adjacencyFeatureWidth", 20},
                                     {"highlightRegionMargin", -10d},
                                     {"highlightRegionStrokeSize", 1d},
                                     {"highlightRegionStrokeColor", PredefinedColorMap.Colors["Red"]}
                                 };

            _displayController.SetShowCentromere(ShowCentromere);
            _displayController.SetShowHeterochromatin(ShowHeterochromatin);
            _displayController.SetShowFeatureDensitySparkline(ShowAdjacencyScore);
            _displayController.SetShowScale(ShowScale);
        }

        public void ResetToDefaults()
        {
            ShowCentromere = (bool) _defaultValues["showCentromere"];
            ShowHeterochromatin = (bool) _defaultValues["showHeterochromatin"];
            ShowBlockOrientation = (bool) _defaultValues["showBlockOrientation"];
            ShowAdjacencyScore = (bool) _defaultValues["showAdjacencyScore"];
            ShowScale = (bool) _defaultValues["showScale"];
            GenomeLayout = (Orientation) _defaultValues["genomeLayout"];
            ChromosomeLayout = (Orientation) _defaultValues["chromosomeLayout"];
            SynBlocksLayout = (Orientation) _defaultValues["synBlocksLayout"];
            DisplaySize = (double) _defaultValues["displaySize"];
            BlockWidth = (double) _defaultValues["blockWidth"];
            CompGenomeNameFormat = (int) _defaultValues["compGenomeNameFormat"];
            LabelSize = (int) _defaultValues["labelSize"];
            CentromereBgColor = (Color) _defaultValues["centromereBgColor"];
            HeterochromatinBgColor = (Color) _defaultValues["heterochromatinBgColor"];
            GenomeInsideBgColor = (Color) _defaultValues["genomeInsideBgColor"];
            FeatureDensityBgColor = (Color) _defaultValues["featureDensityBgColor"];
            FeatureDensityFillColor = (Color) _defaultValues["featureDensityFillColor"];
            SparklineColor = (Color) _defaultValues["sparklineColor"];
            DataPointFillColor = (Color) _defaultValues["dataPointFillColor"];
            SearchHighlightColor = (Color) _defaultValues["searchHighlightColor"];
            ConservedSyntenyHighlightColor = (Color) _defaultValues["conservedSyntenyHighlightColor"];
            BreakpointClassificationHighlightColor = (Color) _defaultValues["breakpointClassificationHighlightColor"];
            BreakpointClassificationMaxThreshold = (double) _defaultValues["breakpointClassificationMaxThreshold"];
            AdjacencyFeatureWidth = (int) _defaultValues["adjacencyFeatureWidth"];
            HighlightRegionMargin = (double) _defaultValues["highlightRegionMargin"];
            HighlightRegionStrokeSize = (double) _defaultValues["highlightRegionStrokeSize"];
            HighlightRegionStrokeColor = (Color) _defaultValues["highlightRegionStrokeColor"];
        }

        public bool ShowCentromere
        {
            get { return (bool) _appSettings.GetValueOrDefault("showCentromere", _defaultValues["showCentromere"]); }
            set
            {
                var oldValue = ShowCentromere;
                if (oldValue == value) return;

                Debug.WriteLine("ShowCentromere: {0}", value);

                _appSettings.Set("showCentromere", value);
                NotifyPropertyChanged(() => ShowCentromere);

                _displayController.SetShowCentromere(value);
            }
        }

        public bool ShowHeterochromatin
        {
            get { return (bool) _appSettings.GetValueOrDefault("showHeterochromatin", _defaultValues["showHeterochromatin"]); }
            set
            {
                var oldValue = ShowHeterochromatin;
                if (oldValue == value) return;

                Debug.WriteLine("ShowHeterochromatin: {0}", value);

                _appSettings.Set("showHeterochromatin", value);
                NotifyPropertyChanged(() => ShowHeterochromatin);

                _displayController.SetShowHeterochromatin(value);
            }
        }

        public bool ShowBlockOrientation
        {
            get { return (bool) _appSettings.GetValueOrDefault("showBlockOrientation", _defaultValues["showBlockOrientation"]); }
            set
            {
                var oldValue = ShowBlockOrientation;
                if (oldValue == value) return;

                Debug.WriteLine("ShowBlockOrientation: {0}", value);

                _appSettings.Set("showBlockOrientation", value);
                NotifyPropertyChanged(() => ShowBlockOrientation);
                EventPublisher.Publish(new ShowBlockOrientationEvent { ShowBlockOrientation = value });
            }
        }

        public bool ShowAdjacencyScore
        {
            get { return (bool)_appSettings.GetValueOrDefault("showAdjacencyScore", _defaultValues["showAdjacencyScore"]); }
            set
            {
                var oldValue = ShowAdjacencyScore;
                if (oldValue == value) return;

                Debug.WriteLine("ShowAdjacencyScore: {0}", value);

                _appSettings.Set("showAdjacencyScore", value);
                NotifyPropertyChanged(() => ShowAdjacencyScore);
                
                _displayController.SetShowFeatureDensitySparkline(value);
            }
        }

        public bool ShowScale
        {
            get { return (bool)_appSettings.GetValueOrDefault("showScale", _defaultValues["showScale"]); }
            set
            {
                var oldValue = ShowScale;
                if (oldValue == value) return;

                Debug.WriteLine("ShowScale: {0}", value);

                _appSettings.Set("showScale", value);
                NotifyPropertyChanged(() => ShowScale);

                _displayController.SetShowScale(value);
            }
        }

        public Orientation GenomeLayout
        {
            get { return (Orientation) _appSettings.GetValueOrDefault("genomeLayout", _defaultValues["genomeLayout"]); }
            set
            {
                var oldValue = GenomeLayout;
                if (oldValue == value) return;

                Debug.WriteLine("SetGenomeLayout: {0}", value);

                _appSettings.Set("genomeLayout", value);
                NotifyPropertyChanged(() => GenomeLayout);
                EventPublisher.Publish(new SetGenomeLayoutEvent{ Layout = value });
            }
        }

        public Orientation ChromosomeLayout
        {
            get { return (Orientation) _appSettings.GetValueOrDefault("chromosomeLayout", _defaultValues["chromosomeLayout"]); }
            set
            {
                var oldValue = ChromosomeLayout;
                if (oldValue == value) return;

                Debug.WriteLine("SetChromosomeLayout: {0}", value);

                _appSettings.Set("chromosomeLayout", value);
                NotifyPropertyChanged(() => ChromosomeLayout);
                EventPublisher.Publish(new SetChromosomeLayoutEvent { Layout = value });
            }
        }

        public Orientation SynBlocksLayout
        {
            get { return (Orientation)_appSettings.GetValueOrDefault("synBlocksLayout", _defaultValues["synBlocksLayout"]); }
            set
            {
                var oldValue = SynBlocksLayout;
                if (oldValue == value) return;

                Debug.WriteLine("SetSynBlocksLayout: {0}", value);

                _appSettings.Set("synBlocksLayout", value);
                NotifyPropertyChanged(() => SynBlocksLayout);
                EventPublisher.Publish(new SetSynBlocksLayoutEvent { Layout = value });
            }
        }

        public double DisplaySize
        {
            get { return (double)_appSettings.GetValueOrDefault("displaySize", _defaultValues["displaySize"]); }
            set
            {
                Debug.WriteLine("SetDisplaySize: {0}", value);

                _appSettings.Set("displaySize", value);
                NotifyPropertyChanged(() => DisplaySize);
                EventPublisher.Publish(new DisplaySizeChangedEvent { DisplaySize = value });
            }
        }

        public double BlockWidth
        {
            get { return (double)_appSettings.GetValueOrDefault("blockWidth", _defaultValues["blockWidth"]); }
            set
            {
                var oldValue = BlockWidth;
                if (oldValue == value) return;

                Debug.WriteLine("SetBlockWidth: {0}", value);

                _appSettings.Set("blockWidth", value);
                NotifyPropertyChanged(() => BlockWidth);
                EventPublisher.Publish(new BlockWidthChangedEvent { BlockWidth = value });
            }
        }

        public int CompGenomeNameFormat
        {
            get { return (int)_appSettings.GetValueOrDefault("compGenomeNameFormat", _defaultValues["compGenomeNameFormat"]); }
            set
            {
                var oldValue = CompGenomeNameFormat;
                if (oldValue == value) return;

                Debug.WriteLine("CompGenomeNameFormat: {0}", value);

                _appSettings.Set("compGenomeNameFormat", value);
                NotifyPropertyChanged(() => CompGenomeNameFormat);
                EventPublisher.Publish(new CompGenomeNameFormatChangedEvent { CompGenomeNameFormat = value });
            }
        }

        public int LabelSize
        {
            get { return (int)_appSettings.GetValueOrDefault("labelSize", _defaultValues["labelSize"]); }
            set
            {
                var oldValue = LabelSize;
                if (oldValue == value) return;

                Debug.WriteLine("SetLabelSize: {0}", value);

                _appSettings.Set("labelSize", value);
                NotifyPropertyChanged(() => LabelSize);
                EventPublisher.Publish(new LabelSizeChangedEvent { LabelSize = value });
            }
        }

        public Color CentromereBgColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("centromereBgColor", _defaultValues["centromereBgColor"]); }
            set
            {
                var oldValue = CentromereBgColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetCentromereBgColor: {0}", value);

                _appSettings.Set("centromereBgColor", value);
                NotifyPropertyChanged(() => CentromereBgColor);
                EventPublisher.Publish(new CentromereBgColorChangedEvent { Color = value });
            }
        }

        public Color HeterochromatinBgColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("heterochromatinBgColor", _defaultValues["heterochromatinBgColor"]); }
            set
            {
                var oldValue = HeterochromatinBgColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetHeterochromatinBgColor: {0}", value);

                _appSettings.Set("heterochromatinBgColor", value);
                NotifyPropertyChanged(() => HeterochromatinBgColor);
                EventPublisher.Publish(new HeterochromatinBgColorChangedEvent { Color = value });
            }
        }

        public Color GenomeInsideBgColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("genomeInsideBgColor", _defaultValues["genomeInsideBgColor"]); }
            set
            {
                var oldValue = GenomeInsideBgColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetGenomeInsideBgColor: {0}", value);

                _appSettings.Set("genomeInsideBgColor", value);
                NotifyPropertyChanged(() => GenomeInsideBgColor);
                EventPublisher.Publish(new GenomeInsideBgColorChangedEvent { Color = value });
            }
        }

        public Color FeatureDensityBgColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("featureDensityBgColor", _defaultValues["featureDensityBgColor"]); }
            set
            {
                var oldValue = FeatureDensityBgColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetFeatureDensityBgColor: {0}", value);

                _appSettings.Set("featureDensityBgColor", value);
                NotifyPropertyChanged(() => FeatureDensityBgColor);
                EventPublisher.Publish(new FeatureDensityBgColorChangedEvent { Color = value });
            }
        }

        public Color FeatureDensityFillColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("featureDensityFillColor", _defaultValues["featureDensityFillColor"]); }
            set
            {
                var oldValue = FeatureDensityFillColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetFeatureDensityFillColor: {0}", value);

                _appSettings.Set("featureDensityFillColor", value);
                NotifyPropertyChanged(() => FeatureDensityFillColor);
                EventPublisher.Publish(new FeatureDensityFillColorChangedEvent { Color = value });
            }
        }

        public Color SparklineColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("sparklineColor", _defaultValues["sparklineColor"]); }
            set
            {
                var oldValue = SparklineColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetSparklineColor: {0}", value);

                _appSettings.Set("sparklineColor", value);
                NotifyPropertyChanged(() => SparklineColor);
                EventPublisher.Publish(new SparklineColorChangedEvent { Color = value });
            }
        }

        public Color DataPointFillColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("dataPointFillColor", _defaultValues["dataPointFillColor"]); }
            set
            {
                var oldValue = DataPointFillColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetDataPointFillColor: {0}", value);

                _appSettings.Set("dataPointFillColor", value);
                NotifyPropertyChanged(() => DataPointFillColor);
                EventPublisher.Publish(new DataPointFillColorChangedEvent { Color = value });
            }
        }

        public Color SearchHighlightColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("searchHighlightColor", _defaultValues["searchHighlightColor"]); }
            set
            {
                var oldValue = SearchHighlightColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetSearchHighlightColor: {0}", value);

                _appSettings.Set("searchHighlightColor", value);
                NotifyPropertyChanged(() => SearchHighlightColor);
                EventPublisher.Publish(new HighlightColorChangedEvent { Color = value });
            }
        }

        public Color ConservedSyntenyHighlightColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("conservedSyntenyHighlightColor", _defaultValues["conservedSyntenyHighlightColor"]); }
            set
            {
                var oldValue = ConservedSyntenyHighlightColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetConservedSyntenyHighlightColor: {0}", value);

                _appSettings.Set("conservedSyntenyHighlightColor", value);
                NotifyPropertyChanged(() => ConservedSyntenyHighlightColor);
                EventPublisher.Publish(new HighlightColorChangedEvent { Color = value });
            }
        }

        public Color BreakpointClassificationHighlightColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("breakpointClassificationHighlightColor", _defaultValues["breakpointClassificationHighlightColor"]); }
            set
            {
                var oldValue = BreakpointClassificationHighlightColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetBreakpointClassificationHighlightColor: {0}", value);

                _appSettings.Set("breakpointClassificationHighlightColor", value);
                NotifyPropertyChanged(() => BreakpointClassificationHighlightColor);
                EventPublisher.Publish(new HighlightColorChangedEvent { Color = value });
            }
        }

        public double BreakpointClassificationMaxThreshold
        {
            get { return (double)_appSettings.GetValueOrDefault("breakpointClassificationMaxThreshold", _defaultValues["breakpointClassificationMaxThreshold"]); }
            set
            {
                var oldValue = BreakpointClassificationMaxThreshold;
                if (oldValue.Equals(value)) return;

                Debug.WriteLine("SetBreakpointClassificationMaxThreshold: {0}", value);

                _appSettings.Set("breakpointClassificationMaxThreshold", value);
                NotifyPropertyChanged(() => BreakpointClassificationMaxThreshold);
            }
        }

        public int AdjacencyFeatureWidth
        {
            get { return (int)_appSettings.GetValueOrDefault("adjacencyFeatureWidth", _defaultValues["adjacencyFeatureWidth"]); }
            set
            {
                var oldValue = AdjacencyFeatureWidth;
                if (oldValue == value) return;

                Debug.WriteLine("SetAdjacencyFeatureWidth: {0}", value);

                _appSettings.Set("adjacencyFeatureWidth", value);
                NotifyPropertyChanged(() => AdjacencyFeatureWidth);
                EventPublisher.Publish(new AdjacencyFeatureWidthChangedEvent { Width = value });
            }
        }

        public double HighlightRegionMargin
        {
            get { return (double)_appSettings.GetValueOrDefault("highlightRegionMargin", _defaultValues["highlightRegionMargin"]); }
            set
            {
                var oldValue = HighlightRegionMargin;
                if (oldValue == value) return;

                Debug.WriteLine("SetHighlightRegionMargin: {0}", value);

                _appSettings.Set("highlightRegionMargin", value);
                NotifyPropertyChanged(() => HighlightRegionMargin);
            }
        }

        public double HighlightRegionStrokeSize
        {
            get { return (double)_appSettings.GetValueOrDefault("highlightRegionStrokeSize", _defaultValues["highlightRegionStrokeSize"]); }
            set
            {
                var oldValue = HighlightRegionMargin;
                if (oldValue == value) return;

                Debug.WriteLine("SetHighlightRegionStrokeSize: {0}", value);

                _appSettings.Set("highlightRegionStrokeSize", value);
                NotifyPropertyChanged(() => HighlightRegionStrokeSize);
            }
        }

        public Color HighlightRegionStrokeColor
        {
            get { return (Color)_appSettings.GetValueOrDefault("highlightRegionStrokeColor", _defaultValues["highlightRegionStrokeColor"]); }
            set
            {
                var oldValue = HighlightRegionStrokeColor;
                if (oldValue == value) return;

                Debug.WriteLine("SetHighlightRegionStrokeColor: {0}", value);

                _appSettings.Set("highlightRegionStrokeColor", value);
                NotifyPropertyChanged(() => HighlightRegionStrokeColor);
                EventPublisher.Publish(new HighlightRegionStrokeColorChangedEvent { Color = value });
            }
        }
    }
}
