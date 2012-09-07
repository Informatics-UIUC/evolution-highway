using System;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Settings.Models;
using System.Collections.Generic;

namespace EvolutionHighwayApp.Settings.ViewModels
{
    public class BreakpointClassificationOptionsDialogViewModel : ViewModelBase
    {
        public IEnumerable<string> Classes { get; private set; }

        private double _maxThreshold;
        public double MaxThreshold
        {
            get { return _maxThreshold; }
            set
            {
                if (value <= 0)
                    throw new Exception("Please enter a strictly positive number.");

                NotifyPropertyChanged(() => MaxThreshold, ref _maxThreshold, value);
            }
        }

        public BreakpointClassificationOptionsDialogViewModel(BreakpointClassificationOptions options)
        {
            Classes = options.Classes;
            MaxThreshold = options.MaxThreshold / 1e6;
        }
    }
}
