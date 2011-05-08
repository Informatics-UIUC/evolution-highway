using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using EvolutionHighwayWidgets.Converters;

namespace EvolutionHighwayModel
{
    [DataContract]
    public class Chromosome : INotifyPropertyChanged
    {
        [DataMember] public string Name { get; set; }
        [DataMember] public double Length { get; set; }
        [DataMember] public IList<ComparativeSpecies> ComparativeSpecies { get; set; }
        [DataMember] public IList<CentromereRegion> CentromereRegions { get; set; }
        [DataMember] public IList<HeterochromatinRegion> HeterochromatinRegions { get; set; }

        private Visibility _centromereVisibility;
        public Visibility CentromereVisibility
        {
            get { return _centromereVisibility; }
            set
            {
                _centromereVisibility = value;
                OnPropertyChanged("ClipRegion");
                OnPropertyChanged("CentromereVisibility");
            }
        }

        private Visibility _heterochromatinVisibility;
        public Visibility HeterochromatinVisibility
        {
            get { return _heterochromatinVisibility; }
            set
            {
                _heterochromatinVisibility = value;
                OnPropertyChanged("HeterochromatinVisibility");
            }
        }

        public Geometry ClipRegion
        {
            get { return GetClipRegion(); }
        }

        private Geometry GetClipRegion()
        {
            var list = ComparativeSpecies;
            if (list == null || list.Count == 0) return null;

            var chr = this;

            var length = (double)new ScaleConverter().Convert(chr.Length, null, null, null);
            var width = 24 * list.Count;

            if (chr.CentromereRegions == null || chr.CentromereRegions.Count == 0 || chr.CentromereVisibility == Visibility.Collapsed)
            // no centromere
            {
                return new RectangleGeometry { RadiusX = 10, RadiusY = 10, Rect = new Rect(0, 0, width, length) };
            }

            var region = chr.CentromereRegions[0];
            var scaledStart = (double)new ScaleConverter().Convert(region.Start, null, null, null);
            var scaledSpan = (double)new ScaleConverter().Convert(region.Span, null, null, null);

            var pg = new PathGeometry();
            var pf = new PathFigure { IsClosed = true, IsFilled = true, StartPoint = new Point(10, 0) };
            pf.Segments.Add(new LineSegment { Point = new Point(width - 10, 0) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(width, 10), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new LineSegment { Point = new Point(width, scaledStart + scaledSpan / 2d - 10) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(width - 5, scaledStart + scaledSpan / 2d), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(width, scaledStart + scaledSpan / 2d + 10), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new LineSegment { Point = new Point(width, length - 10) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(width - 10, length), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new LineSegment { Point = new Point(10, length) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(0, length - 10), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new LineSegment { Point = new Point(0, scaledStart + scaledSpan / 2d + 10) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(5, scaledStart + scaledSpan / 2d), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(0, scaledStart + scaledSpan / 2d - 10), SweepDirection = SweepDirection.Clockwise });
            pf.Segments.Add(new LineSegment { Point = new Point(0, 10) });
            pf.Segments.Add(new ArcSegment { Size = new Size(10, 10), Point = new Point(10, 0), SweepDirection = SweepDirection.Clockwise });
            pg.Figures.Add(pf);

            return pg;
        }

        public Genome Genome { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
