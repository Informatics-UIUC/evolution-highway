using System.Windows;
using EvolutionHighwayApp.Display.ViewModels;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Display.Views
{
    public partial class CustomTrackViewer
    {
        public static readonly DependencyProperty CustomTrackProperty =
            DependencyProperty.Register("CustomTrack", typeof(CustomTrackRegion), typeof(CustomTrackViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((CustomTrackViewer)o).ViewModel.CustomTrack = (CustomTrackRegion)e.NewValue)));

        public CustomTrackRegion CustomTrack
        {
            get { return (CustomTrackRegion)GetValue(CustomTrackProperty); }
            set { SetValue(CustomTrackProperty, value); }
        }

        private CustomTrackViewModel ViewModel
        {
            get { return DataContext as CustomTrackViewModel; }
        }

        public CustomTrackViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new CustomTrackViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }
    }
}
