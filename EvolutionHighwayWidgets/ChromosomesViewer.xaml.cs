using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace EvolutionHighwayWidgets
{
    public partial class ChromosomesViewer : UserControl
    {
        public static readonly DependencyProperty GenomeNameProperty =
            DependencyProperty.Register("GenomeName", typeof(string), typeof(ChromosomesViewer), null);

        public string GenomeName
        {
            get { return (string)GetValue(GenomeNameProperty); }
            set { SetValue(GenomeNameProperty, value); }
        }

        public ChromosomesViewer()
        {
            InitializeComponent();
        }
    }
}
