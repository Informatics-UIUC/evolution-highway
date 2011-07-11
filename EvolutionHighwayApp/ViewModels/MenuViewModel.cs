using System.Diagnostics;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.Commands;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public AppSettings AppSettings { get; set; }

        public Command CaptureScreenCommand { get; private set; }
        public Command ResetZoomCommand { get; private set; }

        #endregion

        public MenuViewModel(IEventPublisher eventPublisher) 
            : base(eventPublisher)
        {
            CaptureScreenCommand = new Command(CaptureScreen, canExecute => true);
            ResetZoomCommand = new Command(ResetZoom, canExecute => true);
        }

        private void CaptureScreen(object param)
        {
            Debug.WriteLine("CaptureScreen invoked");
            RefGenomeCollectionViewModel.CaptureScreen();
            EventPublisher.Publish(new CaptureScreenEvent());
        }

        private void ResetZoom(object param)
        {
            Debug.WriteLine("ResetZoom invoked");
            AppSettings.BlockWidth = 24d;
            AppSettings.DisplaySize = 500d;
        }
    }
}
