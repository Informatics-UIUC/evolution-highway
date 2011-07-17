﻿using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using System.Windows.Media;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Utils;
using SilverlightColorPicker;

namespace EvolutionHighwayApp
{
    public class AppSettings : ViewModelBase
    {
        private readonly IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;

        public AppSettings(IEventPublisher eventPublisher) 
            : base(eventPublisher) { }

        public bool ShowCentromere
        {
            get { return (bool) _appSettings.GetValueOrDefault("showCentromere", true); }
            set
            {
                var oldValue = ShowCentromere;
                if (oldValue == value) return;

                Debug.WriteLine("ShowCentromere: {0}", value);

                _appSettings.Set("showCentromere", value);
                NotifyPropertyChanged(() => ShowCentromere);
                EventPublisher.Publish(new ShowCentromereEvent { ShowCentromere = value });
            }
        }

        public bool ShowHeterochromatin
        {
            get { return (bool) _appSettings.GetValueOrDefault("showHeterochromatin", true); }
            set
            {
                var oldValue = ShowHeterochromatin;
                if (oldValue == value) return;

                Debug.WriteLine("ShowHeterochromatin: {0}", value);

                _appSettings.Set("showHeterochromatin", value);
                NotifyPropertyChanged(() => ShowHeterochromatin);
                EventPublisher.Publish(new ShowHeterochromatinEvent { ShowHeterochromatin = value });
            }
        }

        public bool ShowBlockOrientation
        {
            get { return (bool) _appSettings.GetValueOrDefault("showBlockOrientation", false); }
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
            get { return (bool)_appSettings.GetValueOrDefault("showAdjacencyScore", false); }
            set
            {
                var oldValue = ShowAdjacencyScore;
                if (oldValue == value) return;

                Debug.WriteLine("ShowAdjacencyScore: {0}", value);

                _appSettings.Set("showAdjacencyScore", value);
                NotifyPropertyChanged(() => ShowAdjacencyScore);
                EventPublisher.Publish(new ShowAdjacencyScoreEvent { ShowAdjacencyScore = value });
            }
        }

        public Orientation GenomeLayout
        {
            get { return (Orientation) _appSettings.GetValueOrDefault("genomeLayout", Orientation.Horizontal); }
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
            get { return (Orientation) _appSettings.GetValueOrDefault("chromosomeLayout", Orientation.Horizontal); }
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
            get { return (Orientation) _appSettings.GetValueOrDefault("synBlocksLayout", Orientation.Vertical); }
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
            get { return (double) _appSettings.GetValueOrDefault("displaySize", 500d); }
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
            get { return (double) _appSettings.GetValueOrDefault("blockWidth", 24d); }
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
            get { return (int) _appSettings.GetValueOrDefault("compGenomeNameFormat", 7); }
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
            get { return (int) _appSettings.GetValueOrDefault("labelSize", 55); }
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
            get { return (Color)_appSettings.GetValueOrDefault("centromereBgColor", Colors.Black); }
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
            get { return (Color) _appSettings.GetValueOrDefault("heterochromatinBgColor", Colors.Transparent); }
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
            get { return (Color) _appSettings.GetValueOrDefault("genomeInsideBgColor", Colors.White); }
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
            get { return (Color)_appSettings.GetValueOrDefault("featureDensityBgColor", PredefinedColors.AllColors["AliceBlue"]); }
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
            get { return (Color)_appSettings.GetValueOrDefault("featureDensityFillColor", Colors.Transparent); }
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
            get { return (Color)_appSettings.GetValueOrDefault("sparklineColor", Colors.Black); }
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
            get { return (Color)_appSettings.GetValueOrDefault("dataPointFillColor", PredefinedColors.AllColors["LightSalmon"]); }
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

        public int AdjacencyFeatureWidth
        {
            get { return (int)_appSettings.GetValueOrDefault("adjacencyFeatureWidth", 20); }
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
    }
}
