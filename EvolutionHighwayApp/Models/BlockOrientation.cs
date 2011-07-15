﻿using EvolutionHighwayApp.State;

namespace EvolutionHighwayApp.Models
{
    public class BlockOrientation 
    {
        public double BlockWidth { get; set; }

        public double TopOffset
        {
            get { return (_start.HasValue && _compChrLength > 0) ? BlockWidth * _start.Value / _compChrLength : 0; }
        }

        public double BottomOffset
        {
            get { return (_end.HasValue && _compChrLength > 0) ? BlockWidth * _end.Value / _compChrLength : 0; }
        }

        private double? _start;
        private double? _end;
        private readonly double _compChrLength;


        public BlockOrientation(SyntenyRegion syntenyRegion, double blockWidth)
        {
            BlockWidth = blockWidth;

            _start = (syntenyRegion.Sign == -1) ? syntenyRegion.ModEnd : syntenyRegion.ModStart;
            _end = (syntenyRegion.Sign == -1) ? syntenyRegion.ModStart : syntenyRegion.ModEnd;
            _compChrLength = Repository.CompChromosomeLengths[syntenyRegion.CompGenome.Name][syntenyRegion.Chromosome];
        }
    }
}