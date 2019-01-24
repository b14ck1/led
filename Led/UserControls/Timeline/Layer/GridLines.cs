using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Led.UserControls.Timeline.Layer
{
    class GridLines : Canvas
    {
        public TimeSpan TimelineLength
        {
            get => _TimelineLength;
            set
            {
                if (_TimelineLength != value)
                {
                    _TimelineLength = value;
                    _UpdateViewLevel();
                    _DrawLines();
                }
            }
        }
        private TimeSpan _TimelineLength;
        
        public double LineWidthNormal
        {
            get => _LineWidthNormal;
            set
            {
                if (_LineWidthNormal != value)
                {
                    _LineWidthNormal = value;
                    _UpdateGridLineParameters();
                    _DrawLines();
                }
            }
        }
        private double _LineWidthNormal;
        
        public double LineWidthBold
        {
            get => _LineWidthBold;
            set
            {
                if (_LineWidthBold != value)
                {
                    _LineWidthBold = value;
                    _UpdateGridLineParameters();
                    _DrawLines();
                }
            }
        }
        private double _LineWidthBold;

        public Brush LineBrushNormal
        {
            get => _LineBrushNormal;
            set
            {
                if(_LineBrushNormal != value)
                {
                    _LineBrushNormal = value;
                    _DrawLines();
                }
            }
        }
        private Brush _LineBrushNormal;

        public Brush LineBrushBold
        {
            get => _LineBrushBold;
            set
            {
                if (_LineBrushBold != value)
                {
                    _LineBrushBold = value;
                    _DrawLines();
                }
            }
        }
        private Brush _LineBrushBold;

        public GridLineParameters GridLineParameters { get; private set; }

        private TimelineViewLevel _TimelineViewLevel;        

        public GridLines()
        {
            GridLineParameters = new GridLineParameters();

            //Set Defaults
            _TimelineLength = new TimeSpan(0);
            _LineWidthNormal = 2;
            _LineWidthBold = 4;
            _LineBrushNormal = Brushes.LightGray;
            _LineBrushBold = Brushes.LightCoral;
        }

        public void Update(double width, double height)
        {
            //Set new Width and Height
            Width = width;
            Height = height;            

            //Update the layout so the ActualWidth and ActualHeight are calculated properly
            UpdateLayout();

            _UpdateViewLevel();
            _DrawLines();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Update(ActualWidth, ActualHeight);
        }

        private void _DrawLines()
        {
            for (int i = 0; i < GridLineParameters.NumLines; i++)
            {
                if (Children.Count < i + 1)
                    Children.Add(new Rectangle());

                if(i % GridLineParameters.ModuloBoldLines == 0)
                {
                    ((Rectangle)Children[i]).Width = _LineWidthBold;                    
                    ((Rectangle)Children[i]).Fill = _LineBrushNormal;
                }
                else
                {
                    ((Rectangle)Children[i]).Width = _LineWidthNormal;                    
                    ((Rectangle)Children[i]).Fill = _LineBrushNormal;
                }

                ((Rectangle)Children[i]).Height = ActualHeight;
                SetLeft((Rectangle)Children[i], i * GridLineParameters.LineSpacing);
            }

            if (Children.Count > GridLineParameters.NumLines)
                Children.RemoveRange(GridLineParameters.NumLines, Children.Count - GridLineParameters.NumLines);
        }

        private void _UpdateViewLevel()
        {
            double _pixelPerMilliseconds = ActualWidth / TimelineLength.TotalMilliseconds;

            if (_pixelPerMilliseconds >= 0.035)
                _TimelineViewLevel = TimelineViewLevel.Seconds;
            else if (_pixelPerMilliseconds >= 0.02)
                _TimelineViewLevel = TimelineViewLevel.QuarterMinutes;
            else if (_pixelPerMilliseconds >= 0.01)
                _TimelineViewLevel = TimelineViewLevel.HalfMinutes;

            _UpdateGridLineParameters();
        }

        private void _UpdateGridLineParameters()
        {
            switch (_TimelineViewLevel)
            {
                case TimelineViewLevel.Minutes:
                    GridLineParameters.Update(6, 10000, TimelineLength, ActualWidth);
                    break;
                case TimelineViewLevel.HalfMinutes:
                    GridLineParameters.Update(6, 5000, TimelineLength, ActualWidth);
                    break;
                case TimelineViewLevel.QuarterMinutes:
                    GridLineParameters.Update(6, 2500, TimelineLength, ActualWidth);
                    break;
                case TimelineViewLevel.Seconds:
                    GridLineParameters.Update(10, 1000, TimelineLength, ActualWidth);
                    break;
                case TimelineViewLevel.Frames:
                    GridLineParameters.Update(6, 5000, TimelineLength, ActualWidth);
                    break;
                default:
                    break;
            }
        }
    }

    internal class GridLineParameters
    {
        public int NumLines { get; private set; }
        public double LineSpacing { get; private set; }
        public double ModuloBoldLines { get; private set; }
        public long MillisecondsBetweenLines { get; private set; }
        public double PixelsPerMillisecond { get; private set; }

        public void Update(int moduloBoldLines, long millisecondsBetweenLines, TimeSpan length, double availableWidth)
        {
            ModuloBoldLines = moduloBoldLines;
            MillisecondsBetweenLines = millisecondsBetweenLines;            

            //Total number of lines we need to draw, the "0"-Line is not included here
            NumLines = (int)(length.TotalMilliseconds / MillisecondsBetweenLines);

            LineSpacing = MillisecondsBetweenLines / length.TotalMilliseconds * availableWidth;

            PixelsPerMillisecond = LineSpacing / millisecondsBetweenLines;

            //Add one more line so the "Zero-Line" can be drawn
            NumLines++;
        }
    }
}
