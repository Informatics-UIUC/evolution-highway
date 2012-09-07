using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.Selection.Models
{
    public class SelectableItem : ModelBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { NotifyPropertyChanged(() => IsSelected, ref _isSelected, value); }
        }

        public SelectableItem()
        {
            IsSelected = false;
        }
    }
}
