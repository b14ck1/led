using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Led.UserControls.Timeline.Layer
{
    class GridLineTooltips : Canvas
    {
        public GridLineParameters GridLineParameters { get; private set; }
                
        public GridLineTooltips(GridLineParameters gridLineParameters)
        {
            GridLineParameters = gridLineParameters;
            Height = 20;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Background = Brushes.AliceBlue;
        }

        public void Update()
        {
            if (GridLineParameters == null)
                return;
            
            int _numTooltips = (int)(GridLineParameters.NumLines / GridLineParameters.ModuloBoldLines) + 1;
            for (int i = 0; i < _numTooltips; i++)
            {
                if (Children.Count < i + 1)
                {
                    Children.Add(new TimelineTimeTooltipUserControl(TimeSpan.FromSeconds(0)));
                    ((TimelineTimeTooltipUserControl)Children[i]).MaxHeight = 20;
                    SetBottom(((TimelineTimeTooltipUserControl)Children[i]), 0);
                }

                ((TimelineTimeTooltipUserControl)Children[i]).Time = TimeSpan.FromMilliseconds(GridLineParameters.ModuloBoldLines * i * GridLineParameters.MillisecondsBetweenLines);
                ((TimelineTimeTooltipUserControl)Children[i]).UpdateLayout();
                ((TimelineTimeTooltipUserControl)Children[i]).XOffset = GridLineParameters.ModuloBoldLines * i * GridLineParameters.LineSpacing - ((TimelineTimeTooltipUserControl)Children[i]).ActualWidth / 2;
            }

            if (Children.Count > _numTooltips)
                Children.RemoveRange(_numTooltips, Children.Count - _numTooltips);
        }

        public void UpdateScrolling(double offset)
        {
            Height = 20 + offset;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Update();
        }
    }
}
