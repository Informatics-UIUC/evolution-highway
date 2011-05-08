using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class AncestorRegion : AncestralRegion, INotifyPropertyChanged
    {
        [DataMember] public string Label { get; set; }
        [DataMember] public int? Sign { get; set; }
        [DataMember] public double? ModStart { get; set; }
        [DataMember] public double? ModEnd { get; set; }
        [DataMember] public string Chromosome { get; set; }

        public ComparativeSpecies ComparativeSpecies { get; set; }

        public double? ModSpan
        {
            get { return (ModStart.HasValue && ModEnd.HasValue) ? ModEnd - ModStart : null; }
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
                var chromosomeLength = SpeciesChromosomeLengths.ChromosomeLengths[ComparativeSpecies.SpeciesName][Chromosome];
                var x1 = (start.HasValue && chromosomeLength > 0) ? 
                    24 * start.Value / chromosomeLength : 0;
                return x1;
            }
        }
        
        public double X2
        {
            get
            {
                var end = (Sign == -1) ? ModStart : ModEnd;
                var chromosomeLength = SpeciesChromosomeLengths.ChromosomeLengths[ComparativeSpecies.SpeciesName][Chromosome];
                var x2 = (end.HasValue && chromosomeLength > 0) ?
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
