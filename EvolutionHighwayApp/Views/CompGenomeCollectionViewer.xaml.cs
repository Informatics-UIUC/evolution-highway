using System.Windows;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class CompGenomeCollectionViewer
    {
        public static readonly DependencyProperty RefChromosomeProperty =
            DependencyProperty.Register("RefChromosome", typeof(RefChromosome), typeof(CompGenomeCollectionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((CompGenomeCollectionViewer)o).ViewModel.RefChromosome = (RefChromosome)e.NewValue)));

        public static readonly DependencyProperty BlockWidthProperty =
            DependencyProperty.Register("BlockWidth", typeof(int), typeof(CompGenomeCollectionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((CompGenomeCollectionViewer)o).ViewModel.BlockWidth = (int)e.NewValue)));




        public RefChromosome RefChromosome
        {
            get { return (RefChromosome)GetValue(RefChromosomeProperty); }
            set { SetValue(RefChromosomeProperty, value); }
        }

        public int BlockWidth
        {
            get { return (int) GetValue(BlockWidthProperty); }
            set { SetValue(BlockWidthProperty, value);}
        }

        private CompGenomeCollectionViewModel ViewModel
        {
            get { return DataContext as CompGenomeCollectionViewModel; }
        }

        public CompGenomeCollectionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = IoC.Container.Resolve<CompGenomeCollectionViewModel>();
            Unloaded += delegate { ViewModel.Dispose(); IoC.Container.Release(ViewModel); };

            LayoutRoot.SizeChanged += new System.Windows.SizeChangedEventHandler(LayoutRoot_SizeChanged);
            LayoutRoot.LayoutUpdated += new System.EventHandler(LayoutRoot_LayoutUpdated);
        }

        //FIX ME
        void LayoutRoot_LayoutUpdated(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
            AppSettings asdd = IoC.Container.Resolve<AppSettings>();
            double hack_sc = asdd.DisplaySize;
            //System.Diagnostics.Debug.WriteLine("LayoutRoot_LayoutUpdated: {0} {1}", hack_sc, LayoutRoot.RowDefinitions[1].ActualHeight);
           // MySparkLine.SetWidthHeight(BlockWidth, hack_sc, 0); //Layout

        }

        //FIX ME BORIS I KNOW ITS BAD, JUST A TEMPORARY MEASURE
        //Problem is getting actual size of rendered window
        void LayoutRoot_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            //
            AppSettings asdd = IoC.Container.Resolve<AppSettings>();
            double hack_sc = asdd.DisplaySize;

            //MySparkLine.SetWidthHeight(MySparkLine.ActualWidth, MySparkLine.ActualHeight);
           // MySparkLine.SetWidthHeight(BlockWidth, MySparkLine.ActualHeight, LayoutRoot.RowDefinitions[0].ActualHeight);
           // MySparkLine.SetWidthHeight(BlockWidth, LayoutRoot.RowDefinitions[1].ActualHeight, LayoutRoot.RowDefinitions[0].ActualHeight);
            // MySparkLine.SetWidthHeight(BlockWidth, LayoutRoot.RowDefinitions[1].ActualHeight);
           // MySparkLine.SetWidthHeight(BlockWidth, MySparkLine.ActualHeight - LayoutRoot.RowDefinitions[0].ActualHeight);
           MySparkLine.SetWidthHeight(BlockWidth, hack_sc, 0); //LayoutRoot.RowDefinitions[0].ActualHeight);
           // MySparkLine.SetWidthHeight(BlockWidth, LayoutRoot.RowDefinitions[1].ActualHeight, 0); //LayoutRoot.RowDefinitions[0].ActualHeight);
           // System.Diagnostics.Debug.WriteLine("LayoutRoot_SizeChanged: {0} {1}", hack_sc, LayoutRoot.RowDefinitions[1].ActualHeight);


        }

    }
}
