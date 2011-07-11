using System;
using System.Collections.Generic;
using System.Windows.Controls.DataVisualization;
using System.Windows.Media;

namespace EvolutionHighwayApp.Utils
{
    public class LinearGradientBrushInterpolator : Interpolator
    {
        List<SolidColorBrushInterpolator> interpolators = new List<SolidColorBrushInterpolator>();
        LinearGradientBrush templateBrush;

        public LinearGradientBrush TemplateBrush
        {
            get
            {
                return templateBrush;
            }
            set
            {
                if (value != templateBrush)
                {
                    templateBrush = value;
                    interpolators.Clear();
                }
            }
        }
        private void InitializeBrush(LinearGradientBrush br)
        {
            if (br == null)
                throw new ArgumentNullException("br");
            //set start and end interpolators
            double start = br.StartPoint.X;
            double end = br.EndPoint.X;

            if (br.GradientStops.Count < 2)
            {
                throw new Exception("2 Gradiant Stops must exist");
            }


            SolidColorBrushInterpolator startInterpolator = new SolidColorBrushInterpolator();
            startInterpolator.DataMinimum = start;
            startInterpolator.DataMaximum = br.GradientStops[1].Offset;
            startInterpolator.From = br.GradientStops[0].Color;
            startInterpolator.To = br.GradientStops[1].Color;
            interpolators.Add(startInterpolator);

            for (int i = 1; i < br.GradientStops.Count - 1; i++)
            {
                SolidColorBrushInterpolator inter = new SolidColorBrushInterpolator();
                inter.DataMinimum = br.GradientStops[i].Offset;
                inter.DataMaximum = br.GradientStops[i + 1].Offset;
                inter.From = br.GradientStops[i].Color;
                inter.To = br.GradientStops[i + 1].Color;
                interpolators.Add(inter);
            }

        }
        public override object Interpolate(double value)
        {
            if (interpolators.Count == 0)
            {
                InitializeBrush(templateBrush);
                if (interpolators.Count == 0)
                {
                    throw new InvalidOperationException("TemplateBrush is not optional");
                }
            }
            double min = 0;
            double max = 0;
            if (double.IsNaN(DataMinimum))
            {
                min = ActualDataMinimum;
            }
            if (double.IsNaN(DataMinimum))
            {
                max = ActualDataMaximum;
            }


            if (value < min)
            {
                value = min;
            }
            if (value > max)
            {
                value = max;
            }

            //calculate relative interpolation
            double offset = (value - min) / (max - min);

            foreach (SolidColorBrushInterpolator inter in interpolators)
            {
                if (offset < inter.DataMaximum)
                {
                    if (offset < inter.DataMinimum)
                        offset = inter.DataMinimum;
                    return inter.Interpolate(offset);
                }
            }

            return interpolators[interpolators.Count - 1].Interpolate(interpolators[interpolators.Count - 1].DataMaximum);

        }
    }
}
