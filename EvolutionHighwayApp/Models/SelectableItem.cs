using System;
using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.Models
{
    public class SelectableItem : ModelBase
    {
        public String Name { get; private set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { NotifyPropertyChanged(() => IsSelected, ref _isSelected, value); }
        }

        public SelectableItem(string name)
        {
            Name = name;
            IsSelected = false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
