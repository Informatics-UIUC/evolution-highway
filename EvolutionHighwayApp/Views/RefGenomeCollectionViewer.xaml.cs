using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class RefGenomeCollectionViewer
    {
        private RefGenomeCollectionViewModel ViewModel
        {
            get { return DataContext as RefGenomeCollectionViewModel; }
        }

        public RefGenomeCollectionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            var setSynBlocksLayoutEventObserver = IoC.Container.Resolve<IEventPublisher>().GetEvent<SetSynBlocksLayoutEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnSynBlocksLayoutEvent);

            DataContext = IoC.Container.Resolve<RefGenomeCollectionViewModel>(new { viewer = this });
            Unloaded += delegate { setSynBlocksLayoutEventObserver.Dispose(); ViewModel.Dispose(); IoC.Container.Release(ViewModel); };
        }

        void OnSynBlocksLayoutEvent(SetSynBlocksLayoutEvent e)
        {
            var generator = _itemsControl.ItemContainerGenerator;
            if (generator == null) return;

            foreach (var item in _itemsControl.ItemsSource)
                GetChild<LayoutTransformer>(generator.ContainerFromItem(item)).ApplyLayoutTransform();
        }

        private static T GetChild<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject child = null;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child == null) continue;

                if (child.GetType() == typeof(T))
                    break;

                child = GetChild<T>(child);
                if (child == null) continue;
                
                if (child.GetType() == typeof(T))
                    break;
            }

            return child as T;
        }
    }
}
