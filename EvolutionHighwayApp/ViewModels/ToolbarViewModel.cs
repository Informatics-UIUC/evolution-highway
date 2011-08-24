using System.Diagnostics;
using System.Windows.Controls;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.Commands;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.ViewModels
{
    public class ToolbarViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public AppSettings AppSettings { get; set; }

        public Command ZoomInCommand { get; private set; }
        public Command ZoomOutCommand { get; private set; }
        public Command ResetZoomCommand { get; private set; }

        public Command GenomeOrientationHorizontalCommand { get; private set; }
        public Command GenomeOrientationVerticalCommand { get; private set; }

        #endregion

        public ToolbarViewModel(IEventPublisher eventPublisher) : base(eventPublisher)
        {
            ZoomInCommand = new Command(ZoomIn, canExecute => true);
            ZoomOutCommand = new Command(ZoomOut, canExecute => true);
            ResetZoomCommand = new Command(ResetZoom, canExecute => true);

            GenomeOrientationHorizontalCommand = new Command(GenomeOrientationHorizontal, canExecute => true);
            GenomeOrientationVerticalCommand = new Command(GenomeOrientationVertical, canExecute => true);
        }

        private void ZoomIn(object param)
        {
            Debug.WriteLine("ZoomIn invoked");
            if (AppSettings.DisplaySize + AppSettings.DisplaySizeSmallChange <= AppSettings.DisplaySizeMaximum)
                AppSettings.DisplaySize += AppSettings.DisplaySizeSmallChange;
        }

        private void ZoomOut(object param)
        {
            Debug.WriteLine("ZoomOut invoked");
            if (AppSettings.DisplaySize - AppSettings.DisplaySizeSmallChange >= AppSettings.DisplaySizeMinimum)
                AppSettings.DisplaySize -= AppSettings.DisplaySizeSmallChange;
        }

        private void ResetZoom(object param)
        {
            Debug.WriteLine("ResetZoom invoked");
            EventPublisher.Publish(new ResetZoomEvent());
        }

        private void GenomeOrientationHorizontal(object param)
        {
            AppSettings.SynBlocksLayout = Orientation.Horizontal;
            AppSettings.GenomeLayout = Orientation.Vertical;
            AppSettings.ChromosomeLayout = Orientation.Horizontal;
        }

        private void GenomeOrientationVertical(object param)
        {
            AppSettings.SynBlocksLayout = Orientation.Vertical;
            AppSettings.ChromosomeLayout = Orientation.Horizontal;
            AppSettings.GenomeLayout = Orientation.Horizontal;
        }
    }
}
