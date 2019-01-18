using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Led.UserControls.Timeline
{
    class TimelineLanes : Grid
    {
        private static int _LaneHeight = 30;

        private List<Canvas> _Lanes;

        public TimelineLanes()
        {
            _Lanes = new List<Canvas>();
            
        }

        public void Add()
        {
            RowDefinition r1 = new RowDefinition
            {
                Height = new System.Windows.GridLength(_LaneHeight)
            };
            RowDefinitions.Add(r1);

            Canvas c = new Canvas
            {
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Background = Brushes.Red,
                MinWidth = 200,
                Height = _LaneHeight
            };
            SetRow(c, RowDefinitions.Count - 1);
            _Lanes.Add(c);

            Children.Add(c);
        }

        public void UpdateWidth(int width, double frameToPixelRatio)
        {

        }
    }
}
