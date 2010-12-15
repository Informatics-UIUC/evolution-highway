using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Shapes;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class AncestorRegion : INotifyPropertyChanged
    {
        [DataMember] public double Start { get; set; }
        [DataMember] public double End { get; set; }
        [DataMember] public string Label { get; set; }
        [DataMember] public int? Sign { get; set; }
        [DataMember] public double? ModStart { get; set; }
        [DataMember] public double? ModEnd { get; set; }
        [DataMember] public string Chromosome { get; set; }

        public ComparativeSpecies ComparativeSpecies { get; set; }

        public double Span
        {
            get { return End - Start; }
        }

        private Visibility _lineVisibility;
        public Visibility LineVisibility
        {
            get
            {
                return (_lineVisibility == Visibility.Visible && (X1 != 0d || X2 != 0d)) ?
                    Visibility.Visible : Visibility.Collapsed;
            }
            set
            {
                _lineVisibility = value;
                OnPropertyChanged("LineVisibility");
            }
        }

        public double X1
        {
            get
            {
                var start = (Sign == -1) ? ModEnd : ModStart;
                var spcChrLength = new Tuple<string, string>(ComparativeSpecies.SpeciesName, Chromosome);
                var chromosomeLength = ComparativeSpecies.ChromosomeLengths[spcChrLength];
                var x1 = (start.HasValue && ComparativeSpecies.ChromosomeLengths.ContainsKey(spcChrLength) && chromosomeLength > 0) ? 
                    24 * start.Value / chromosomeLength : 0;
                return x1;
            }
        }
        
        public double X2
        {
            get
            {
                var end = (Sign == -1) ? ModStart : ModEnd;
                var spcChrLength = new Tuple<string, string>(ComparativeSpecies.SpeciesName, Chromosome);
                var chromosomeLength = ComparativeSpecies.ChromosomeLengths[spcChrLength];
                var x2 = (end.HasValue && ComparativeSpecies.ChromosomeLengths.ContainsKey(spcChrLength) && chromosomeLength > 0) ?
                    24 * end.Value / chromosomeLength : 0;
                return x2;
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
