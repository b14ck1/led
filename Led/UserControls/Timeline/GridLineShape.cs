using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Led.UserControls.Timeline
{
    class GridLineShape
    {
        public Brush Brush { get; set; }
        public double LineWidth { get; set; }

        public GridLineShape(Brush brush, double lineWidth)
        {
            Brush = brush;
            LineWidth = lineWidth;
        }
    }
}
