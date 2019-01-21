using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Led.UserControls.Timeline.Layer
{
    class GridLines : Canvas
    {
        public GridLines()
        {

        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
        }
    }

    internal class GridLineParameters
    {
        public int NumLines { get; set; }
        public int ModuloBoldLines { get; private set; }
        public long MillisecondsBetweenLines { get; private set; }
        public double LineSpacing { get; set; }
        public double StartOffset { get; private set; }
        public double WidthTillLastLine { get; set; }

        public GridLineParameters(int moduloBoldLines, long millisecondsBetweenLines)
        {
            ModuloBoldLines = moduloBoldLines;
            MillisecondsBetweenLines = millisecondsBetweenLines;
        }

        public void Calc(TimeSpan length, double availableWidth, double lineWidthNormal, double lineWidthBold)
        {
            NumLines = (int)(length.TotalMilliseconds / MillisecondsBetweenLines);
            StartOffset = lineWidthBold / 2;

            bool _isLastLineBold = NumLines % ModuloBoldLines == 0 ? true : false;
            double _lastLineNeededSpace = _isLastLineBold ? lineWidthBold / 2 : lineWidthNormal / 2;
            double _endWithWithoutLines = length.TotalMilliseconds % MillisecondsBetweenLines;
            double _endOffset = 0;
            if (_endWithWithoutLines <= _lastLineNeededSpace)
                _endOffset = _lastLineNeededSpace - _endWithWithoutLines;

            LineSpacing = MillisecondsBetweenLines / length.TotalMilliseconds * (availableWidth - StartOffset - _endOffset);

            //Add one more line so the "Zero-Line" can be drawn
            NumLines++;
        }
    }
}
