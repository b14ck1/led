using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Led.UserControls.Timeline.Items.Interior
{
    class SetColor : Canvas
    {
        private Rectangle _ColorDisplay;

        public SetColor()
        {
            _ColorDisplay = new Rectangle();
            _ColorDisplay.Width = TimelineItem.ColorDisplaySize;
            _ColorDisplay.Height = TimelineItem.ColorDisplaySize;
            _ColorDisplay.Fill = Brushes.Blue;
        }
    }
}
