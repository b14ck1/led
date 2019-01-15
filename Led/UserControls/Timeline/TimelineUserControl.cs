using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Led.Interfaces;

namespace Led.UserControls.Timeline
{
    internal enum TimelineViewLevel
    {
        Minutes,
        HalfMinutes,
        QuarterMinutes,
        Seconds,
        Frames
    }

    class TimelineUserControl : Canvas, IParticipant
    {
        private IMediator _Mediator;
        private TimelineViewLevel _TimelineViewLevel;

        /// <summary>
        /// One main grid to rule them all.
        /// Divides the area into two vertical sections.
        /// |   StaticTooltips  | ZoomSlider |
        /// |    ScrollViewer   | ZoomSlider |
        /// </summary>
        private Grid _MainGrid;

        /// <summary>
        /// Holds the static Tooltips on top of the ScrollViewer.
        /// </summary>
        private Canvas _StaticTooltipCanvas;
        /// <summary>
        /// Wrapper for the height of the tooltips.
        /// </summary>
        private double _TooltipHeight { get => _StaticTooltipCanvas.ActualHeight; set { _StaticTooltipCanvas.Height = value; _DynamicTooltipCanvas.Height = value; } }

        /// <summary>
        /// Holds the LineGridCanvas and dynamic Tooltips.
        /// </summary>
        private ScrollViewer _ScrollViewer;

        private Grid _ScrollViewerContentWrapper;
        /// <summary>
        /// Holds the dynamic Tooltips above the LineGridCanvas.
        /// </summary>
        private Canvas _DynamicTooltipCanvas;
        /// <summary>
        /// Holds all lines for our timeline.
        /// </summary>
        private Canvas _LineGridCanvas;
        /// <summary>
        /// Offset where the real Children start (the lines seen in the Background) not some Mouse-Shit
        /// </summary>
        private int _LineGridCanvasChildrenOffset;

        /// <summary>
        /// ZoomSlider to zoom the timeline.
        /// </summary>
        private Slider _ZoomSlider;

        /// <summary>
        /// Tooltips at the beginning and end of the ScrollViewer
        /// </summary>
        private TimelineTimeTooltipUserControl[] _StaticTooltips;
        /// <summary>
        /// Tooltip to show when the mouse is hovered over the GridLines
        /// </summary>
        private TimelineTimeTooltipUserControl _MouseTooltip;

        private Rectangle _MouseTooltipLine;

        private double _lineWidthNormal = 2;
        private double _lineWidthBold = 4;

        private bool _IsZoomWithMouseWheel_OnLineGrid;
        private Point _MousePosition;

        /// <summary>
        /// Items to display.
        /// </summary>
        private ObservableCollection<ViewModels.EffectBaseVM> _EffectBaseVMs;
        /// <summary>
        /// When two items would overlap we would draw them on two different lanes.
        /// E.g.: this would bet set to two.
        /// </summary>
        private int _LanesToDraw;
        /// <summary>
        /// On which lane is the item drawn.
        /// </summary>
        private Dictionary<ViewModels.EffectBaseVM, int> _LaneMapping;

        /// <summary>Gets or sets the total time of the timeline.</summary>
        /// <value>Time as TimeSpan this element should display.</value>
        /// <remarks><param>Must be the same as coupled TimelineUserControls. If not the behaviour is undefined.</param></remarks>
        public TimeSpan TimelineLength
        {
            get => (TimeSpan)GetValue(TimelineLengthProperty);
            set { SetValue(TimelineLengthProperty, value); }
        }
        /// <dpdoc />
        public static readonly DependencyProperty TimelineLengthProperty =
            DependencyProperty.Register(nameof(TimelineLength), typeof(TimeSpan), typeof(TimelineUserControl),
                new UIPropertyMetadata(TimeSpan.FromSeconds(120), new PropertyChangedCallback(_OnTimelineLengthChanged)));
        private static void _OnTimelineLengthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>Gets or sets the background image of the timeline.</summary>
        /// <value>Background image of the GridCanvas which represents the background of the timeline.</value>
        /// <remarks><param>Default null.</param></remarks>
        public ImageSource BackgroundImage
        {
            get => (ImageSource)GetValue(BackgroundImageProperty);
            set { SetValue(BackgroundImageProperty, value); }
        }
        /// <dpdoc />
        public static readonly DependencyProperty BackgroundImageProperty =
            DependencyProperty.Register(nameof(BackgroundImage), typeof(ImageSource), typeof(TimelineUserControl),
                new UIPropertyMetadata(null, new PropertyChangedCallback(_OnBackgroundImageChanged)));
        private static void _OnBackgroundImageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //(sender as TimelineUserControl).Background = (sender as TimelineUserControl).BackgroundColor;
        }

        /// <summary>Gets or sets the grid line color of the timeline.</summary>
        /// <value>Line color of the grid lines in the GridCanvas.</value>
        /// <remarks><param>Default Brushes.LightGray.</param></remarks>
        public Brush GridlineColor
        {
            get => (Brush)GetValue(GridlineColorProperty);
            set { SetValue(GridlineColorProperty, value); }
        }
        /// <dpdoc />
        public static readonly DependencyProperty GridlineColorProperty =
            DependencyProperty.Register(nameof(GridlineColor), typeof(Brush), typeof(TimelineUserControl),
                new UIPropertyMetadata(Brushes.LightGray, new PropertyChangedCallback(_OnGridlineColorChanged)));
        private static void _OnGridlineColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>Gets or sets the zoom lock of this timeline.</summary>
        /// <value>Bool if the zoom is locked or not.</value>
        /// <remarks><param>If true, this timeline synchronizes its zooming level with timelines coupled through the mediator pattern.</param></remarks>
        public bool ZoomLock
        {
            get => (bool)GetValue(ZoomLockProperty);
            set { SetValue(ZoomLockProperty, value); }
        }
        /// <dpdoc />
        public static readonly DependencyProperty ZoomLockProperty =
            DependencyProperty.Register("ZoomLock", typeof(bool), typeof(TimelineUserControl), new PropertyMetadata(true));

        public TimelineUserControl()
        {
            //_EffectBaseVMs.CollectionChanged += _OnCollectionChanged;
            _LanesToDraw = 1;
            _InitComponents();

            _ZoomSlider.MouseWheel += _ZoomSlider_MouseWheel;
            _LineGridCanvas.MouseWheel += _LineGridCanvas_MouseWheel;
            _LineGridCanvas.MouseMove += _LineGridCanvas_MouseMove;
            _LineGridCanvas.MouseEnter += _LineGridCanvas_MouseEnter;
            _LineGridCanvas.MouseLeave += (object sender, MouseEventArgs e) => { _MouseTooltip.Visibility = Visibility.Hidden; _MouseTooltipLine.Visibility = Visibility.Hidden; };
            _ScrollViewer.ScrollChanged += _ScrollViewer_ScrollChanged;
        }

        private void _LineGridCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            _MouseTooltip.Visibility = Visibility.Visible;
            _MouseTooltipLine.Visibility = Visibility.Visible;
            _MouseTooltipLine.Height = _LineGridCanvas.ActualHeight;
        }

        private void _UpdateLineGridCanvas()
        {
            _LineGridCanvas.Width = _ZoomSlider.Value * (ActualWidth - _MainGrid.ColumnDefinitions[1].ActualWidth);
            _LineGridCanvas.Height = ActualHeight;
            _LineGridCanvas.UpdateLayout();

            GridLineParameters _parameters = _GetGridLineParameters();
            _DrawLinesOnLineGridCanvas(_parameters);
            _UpdateDynamicTooltips(_parameters);
        }

        private void _UpdateDynamicTooltips(GridLineParameters parameters)
        {
            TimelineTimeTooltipUserControl[] _tooltips = new TimelineTimeTooltipUserControl[parameters.NumLines / parameters.ModuloBoldLines + 1];

            for (int i = 0; i < _tooltips.Length; i++)
            {
                if (_DynamicTooltipCanvas.Children.Count >= i + 1)
                    _tooltips[i] = (TimelineTimeTooltipUserControl)_DynamicTooltipCanvas.Children[i];
                else
                {
                    _tooltips[i] = new TimelineTimeTooltipUserControl(TimeSpan.FromSeconds(0));
                    _DynamicTooltipCanvas.Children.Add(_tooltips[i]);
                }

                _tooltips[i].Time = TimeSpan.FromMilliseconds(parameters.ModuloBoldLines * i * parameters.MillisecondsBetweenLines);
                _tooltips[i].UpdateLayout();
                _tooltips[i].XOffset = parameters.ModuloBoldLines * i * parameters.LineSpacing - _tooltips[i].ActualWidth / 2;
            }            

            if (_DynamicTooltipCanvas.Children.Count > _tooltips.Length)
                _DynamicTooltipCanvas.Children.RemoveRange(_tooltips.Length, _DynamicTooltipCanvas.Children.Count - _tooltips.Length);
        }

        private void _UpdateStaticTooltips()
        {
            _StaticTooltips[0].Time = TimeSpan.FromMilliseconds((long)(_ScrollViewer.HorizontalOffset / _LineGridCanvas.ActualWidth * TimelineLength.TotalMilliseconds));
            _StaticTooltips[1].Time = TimeSpan.FromMilliseconds((long)((_ScrollViewer.HorizontalOffset + _ScrollViewer.ActualWidth) / _LineGridCanvas.ActualWidth * TimelineLength.TotalMilliseconds));
        }

        private void _UpdateZoom()
        {
            double _offsetAbsolutToScrollViewer;
            double _offsetPercentageToCanvas;

            //Calculate current Position
            if (_IsZoomWithMouseWheel_OnLineGrid)
                //Keep the timeline focussed on the mouse position
                _offsetAbsolutToScrollViewer = _MousePosition.X - _ScrollViewer.HorizontalOffset;
            else
                //Keep the timeline focussed on the center
                _offsetAbsolutToScrollViewer = _ScrollViewer.ActualWidth / 2;
            _offsetPercentageToCanvas = (_ScrollViewer.HorizontalOffset + _offsetAbsolutToScrollViewer) / _LineGridCanvas.ActualWidth;

            //Update our grid
            _UpdateLineGridCanvas();

            //Scroll to saved position and update layout for further functions
            _ScrollViewer.ScrollToHorizontalOffset(_offsetPercentageToCanvas * _LineGridCanvas.ActualWidth - _offsetAbsolutToScrollViewer);
            _ScrollViewer.UpdateLayout();

            _UpdateStaticTooltips();
        }

        private void _ZoomSlider_ValueChanged(object sender, EventArgs e)
        {
            _UpdateZoom();
        }

        private void _ZoomSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _ZoomSlider.Value += e.Delta * _ZoomSlider.TickFrequency / 240;
            e.Handled = true;
        }

        private void _LineGridCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Zoom relative to where our mouse is pointed
            _IsZoomWithMouseWheel_OnLineGrid = true;
            _MousePosition = e.GetPosition(sender as IInputElement);

            _ZoomSlider.Value += e.Delta * _ZoomSlider.TickFrequency / 240;

            _IsZoomWithMouseWheel_OnLineGrid = false;
            e.Handled = true;
        }

        private void _LineGridCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            double _XPosition = e.GetPosition(sender as IInputElement).X;
            _MouseTooltip.Time = TimeSpan.FromMilliseconds(_XPosition / _LineGridCanvas.ActualWidth * TimelineLength.TotalMilliseconds);
            _MouseTooltip.UpdateLayout();
            _MouseTooltip.XOffset = _XPosition - _ScrollViewer.HorizontalOffset - _MouseTooltip.ActualWidth / 2;

            SetLeft(_MouseTooltipLine, _XPosition - _MouseTooltipLine.ActualWidth / 2);
            //Console.WriteLine(_ScrollViewerContentWrapper.ActualWidth);
            //Console.WriteLine(_XPosition);
        }

        private void _ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange != 0)
                _UpdateStaticTooltips();
        }

        private void _DrawLinesOnLineGridCanvas(GridLineParameters parameters)
        {
            //Rectangle[] _lines = new Rectangle[parameters.NumLines];

            double _drawStartHeight = 0;
            double _drawStopHeight = _LineGridCanvas.ActualHeight;
            double _rectangleHeight = _drawStopHeight - _drawStartHeight;

            for (int i = _LineGridCanvasChildrenOffset; i < parameters.NumLines; i++)
            {
                if (_LineGridCanvas.Children.Count - _LineGridCanvasChildrenOffset < i + 1)
                {
                    _LineGridCanvas.Children.Add(new Rectangle());
                }

                if (i % parameters.ModuloBoldLines == 0)
                {
                    ((Rectangle)_LineGridCanvas.Children[i]).Width = _lineWidthBold;
                    ((Rectangle)_LineGridCanvas.Children[i]).Height = _rectangleHeight;
                    ((Rectangle)_LineGridCanvas.Children[i]).Fill = Brushes.ForestGreen;
                }
                else
                {
                    ((Rectangle)_LineGridCanvas.Children[i]).Width = _lineWidthNormal;
                    ((Rectangle)_LineGridCanvas.Children[i]).Height = _rectangleHeight;
                    ((Rectangle)_LineGridCanvas.Children[i]).Fill = Brushes.LightGray;
                }

                SetTop((Rectangle)_LineGridCanvas.Children[i], _drawStartHeight);
                SetLeft((Rectangle)_LineGridCanvas.Children[i], i * parameters.LineSpacing);
            }

            if (_LineGridCanvas.Children.Count - _LineGridCanvasChildrenOffset > parameters.NumLines)
            {
                _LineGridCanvas.Children.RemoveRange(parameters.NumLines, _LineGridCanvas.Children.Count - parameters.NumLines);
            }
        }

        private GridLineParameters _GetGridLineParameters()
        {
            GridLineParameters _params;

            //Console.WriteLine("Factor: " + _GridLineCanvas.ActualWidth / TimelineLength.TotalMilliseconds);
            double _scaleFactor = _LineGridCanvas.ActualWidth / TimelineLength.TotalMilliseconds;

            if (_scaleFactor >= 0.035)
                _TimelineViewLevel = TimelineViewLevel.Seconds;
            else if (_scaleFactor >= 0.02)
                _TimelineViewLevel = TimelineViewLevel.QuarterMinutes;
            else if (_scaleFactor >= 0.01)
                _TimelineViewLevel = TimelineViewLevel.HalfMinutes;

            switch (_TimelineViewLevel)
            {
                case TimelineViewLevel.Minutes:
                    _params = new GridLineParameters(6, 10000);
                    break;
                case TimelineViewLevel.HalfMinutes:
                    _params = new GridLineParameters(6, 5000);
                    break;
                case TimelineViewLevel.QuarterMinutes:
                    _params = new GridLineParameters(6, 2500);
                    break;
                case TimelineViewLevel.Seconds:
                    _params = new GridLineParameters(10, 1000);
                    break;
                case TimelineViewLevel.Frames:
                    _params = new GridLineParameters(6, 5000);
                    break;
                default:
                    _params = new GridLineParameters(6, 5000);
                    break;
            }
            _params.Calc(TimelineLength, _LineGridCanvas.ActualWidth, _lineWidthNormal, _lineWidthBold);           

            return _params;
        }


        /// <summary>
        /// Issues the TimelineUserControl to scroll to the given frame (centered).
        /// Following method calls will override the previous calls.
        /// </summary>
        /// <param name="frame">Frame to scroll to.</param>
        /// <param name="speed">Speed for scrolling from 0-1 in seconds.</param>
        private void _ScrollToFrame(ushort frame, double speed)
        {

        }

        /// <summary>
        /// Issues the TimelineUserControl to scroll the given pixels.
        /// Following method calls will override the previous calls.
        /// </summary>
        /// <param name="pixels">Positive values for scrolling to the right. Negative values for scrolling to the left.</param>
        /// <param name="speed">Speed for scrolling from 0-1 in seconds.</param>
        private void _ScrollPixel(int pixels, double speed)
        {

        }

        /// <summary>
        /// Issues the TimelineUserControl to zoom to the given level.
        /// Following method calls will override the previous calls.
        /// </summary>
        /// <param name="zoomLevel">ZoomLevel from 0-1. Where 0 is no zoom and 1 is full zoom.</param>
        /// <param name="speed">Speed for zooming from 0-1 in seconds.</param>
        private void _SetZoomLevel(double zoomLevel, double speed)
        {
            _ZoomSlider.Value = zoomLevel * _ZoomSlider.Maximum + 1;
        }

        /// <summary>
        /// Gets called when we add or remove an EffectBaseVM.
        /// Handles add / remove / reset events and updates the view accordingly.
        /// </summary>
        /// <param name="sender">ObservableCollection with EffectBaseVMs.</param>
        /// <param name="e">Add/remove/reset happened.</param>
        private void _OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        protected override void OnRender(DrawingContext dc)
        {           
            base.OnRender(dc);
            _MainGrid.MaxWidth = ((Canvas)_MainGrid.Parent).ActualWidth;
            _MainGrid.MaxHeight = ((Canvas)_MainGrid.Parent).ActualHeight;

            _StaticTooltips[1].XOffset = _MainGrid.ColumnDefinitions[0].ActualWidth - _StaticTooltips[1].ActualWidth;

            _UpdateLineGridCanvas();
        }

        private void _SendMessage(MediatorMessages message, object data)
        {

        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            throw new NotImplementedException();
        }

        private void _InitComponents()
        {
            //MainGrid with 2 columns
            _MainGrid = new Grid();
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(1, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = GridLength.Auto;
            _MainGrid.ColumnDefinitions.Add(c1);
            _MainGrid.ColumnDefinitions.Add(c2);            
            
            //LineGridCanvas inside the ScrollViewer
            _LineGridCanvas = new Canvas();
            _LineGridCanvas.Background = Brushes.DarkSlateGray;

            _DynamicTooltipCanvas = new Canvas();
            //_DynamicTooltipCanvas.Background = Brushes.Red;
            _DynamicTooltipCanvas.VerticalAlignment = VerticalAlignment.Top;
            _DynamicTooltipCanvas.MinHeight = 20;

            //TODO: somehow implement this MouseTooltipLine to not flicker like hell, idk why it does
            _MouseTooltipLine = new Rectangle();
            _MouseTooltipLine.Width = 2;
            _MouseTooltipLine.Fill = Brushes.Red;
            _MouseTooltipLine.Visibility = Visibility.Hidden;
            _LineGridCanvas.Children.Add(_MouseTooltipLine);
            _LineGridCanvasChildrenOffset++;

            _ScrollViewerContentWrapper = new Grid();            
            _ScrollViewerContentWrapper.Children.Add(_LineGridCanvas);
            _ScrollViewerContentWrapper.Children.Add(_DynamicTooltipCanvas);
            

            _ScrollViewer = new ScrollViewer();
            _ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _ScrollViewer.Content = _ScrollViewerContentWrapper;
            Grid.SetRow(_ScrollViewer, 0);
            Grid.SetColumn(_ScrollViewer, 0);
            _MainGrid.Children.Add(_ScrollViewer);

            _StaticTooltips = new TimelineTimeTooltipUserControl[]
            {
                new TimelineTimeTooltipUserControl(TimeSpan.FromSeconds(0)),
                new TimelineTimeTooltipUserControl(TimelineLength)
            };            
            _MouseTooltip = new TimelineTimeTooltipUserControl(TimeSpan.MinValue);
            _MouseTooltip.Visibility = Visibility.Hidden;

            _StaticTooltipCanvas = new Canvas();
            //_StaticTooltipCanvas.Background = Brushes.Red;
            _StaticTooltipCanvas.VerticalAlignment = VerticalAlignment.Top;
            _StaticTooltipCanvas.MinHeight = 20;
            _StaticTooltips.ToList().ForEach(tooltip => _StaticTooltipCanvas.Children.Add(tooltip));            
            _StaticTooltipCanvas.Children.Add(_MouseTooltip);
            _MainGrid.Children.Add(_StaticTooltipCanvas);

            _ZoomSlider = new Slider();
            _ZoomSlider.Orientation = Orientation.Vertical;
            _ZoomSlider.Margin = new Thickness(7, 5, 7, 5);
            _ZoomSlider.Minimum = 1;
            _ZoomSlider.Maximum = 10;
            _ZoomSlider.TickFrequency = 0.5;
            _ZoomSlider.ValueChanged += _ZoomSlider_ValueChanged;
            Grid.SetRow(_ZoomSlider, 0);
            Grid.SetColumn(_ZoomSlider, 1);
            _MainGrid.Children.Add(_ZoomSlider);

            Children.Add(_MainGrid);
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
}
