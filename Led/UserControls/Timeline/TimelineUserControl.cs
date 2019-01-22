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
        private double _TooltipHeight { get => _StaticTooltipCanvas.ActualHeight; set { _StaticTooltipCanvas.Height = value; /*_DynamicTooltipCanvas.Height = value;*/ } }

        /// <summary>
        /// Holds the LineGridCanvas and dynamic Tooltips.
        /// </summary>
        private ScrollViewer _ScrollViewer;

        private Grid _LineGridOverlay;
        private Grid _ScrollViewerContentWrapper;
        /// <summary>
        /// Holds the dynamic Tooltips above the LineGridCanvas.
        /// </summary>
        //private Canvas _DynamicTooltipCanvas;

        private Layer.GridLines _GridLineLayer;
        private Layer.GridLineTooltips _GridLineTooltipLayer;

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

        private bool _IsZoomWithMouseWheel_OnLineGrid;
        private Point _MousePosition;

        /// <summary>
        /// Items to display.
        /// </summary>
        private ObservableCollection<ViewModels.EffectBaseVM> _EffectBaseVMs;
        private List<TimelineItemUserControl> _Items;

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
            _InitComponents();

            _ZoomSlider.MouseWheel += _ZoomSlider_MouseWheel;
            //_LineGridCanvas.MouseWheel += _LineGridCanvas_MouseWheel;
            //_LineGridCanvas.MouseMove += _LineGridCanvas_MouseMove;
            //_LineGridCanvas.MouseEnter += _LineGridCanvas_MouseEnter;
            //_LineGridCanvas.MouseLeave += (object sender, MouseEventArgs e) => { _MouseTooltip.Visibility = Visibility.Hidden; _MouseTooltipLine.Visibility = Visibility.Hidden; };
            _ScrollViewer.ScrollChanged += _ScrollViewer_ScrollChanged;

            _Items = new List<TimelineItemUserControl>();
            _Items.Add(new TimelineItemUserControl(null));
            Children.Add(_Items[0]);
        }

        private void _LineGridCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            _MouseTooltip.Visibility = Visibility.Visible;
            _MouseTooltipLine.Visibility = Visibility.Visible;
            //_MouseTooltipLine.Height = _LineGridCanvas.ActualHeight;
        }

        private void _UpdateGridLineLayer()
        {
            _GridLineLayer.Update(_ZoomSlider.Value * (ActualWidth - _MainGrid.ColumnDefinitions[1].ActualWidth), ActualHeight);            
        }

        private void _UpdateGridLineTooltipLayer()
        {
            _GridLineTooltipLayer.Update();
        }

        //private void _UpdateDynamicTooltips(GridLineParameters parameters)
        //{
        //    TimelineTimeTooltipUserControl[] _tooltips = new TimelineTimeTooltipUserControl[parameters.NumLines / parameters.ModuloBoldLines + 1];

        //    for (int i = 0; i < _tooltips.Length; i++)
        //    {
        //        if (_DynamicTooltipCanvas.Children.Count >= i + 1)
        //            _tooltips[i] = (TimelineTimeTooltipUserControl)_DynamicTooltipCanvas.Children[i];
        //        else
        //        {
        //            _tooltips[i] = new TimelineTimeTooltipUserControl(TimeSpan.FromSeconds(0));
        //            _DynamicTooltipCanvas.Children.Add(_tooltips[i]);
        //        }

        //        _tooltips[i].Time = TimeSpan.FromMilliseconds(parameters.ModuloBoldLines * i * parameters.MillisecondsBetweenLines);
        //        _tooltips[i].UpdateLayout();
        //        _tooltips[i].XOffset = parameters.ModuloBoldLines * i * parameters.LineSpacing - _tooltips[i].ActualWidth / 2;
        //    }            

        //    if (_DynamicTooltipCanvas.Children.Count > _tooltips.Length)
        //        _DynamicTooltipCanvas.Children.RemoveRange(_tooltips.Length, _DynamicTooltipCanvas.Children.Count - _tooltips.Length);
        //}

        private void _UpdateStaticTooltips()
        {
            _StaticTooltips[0].Time = TimeSpan.FromMilliseconds((long)(_ScrollViewer.HorizontalOffset / _GridLineLayer.ActualWidth * TimelineLength.TotalMilliseconds));
            _StaticTooltips[1].Time = TimeSpan.FromMilliseconds((long)((_ScrollViewer.HorizontalOffset + _ScrollViewer.ActualWidth) / _GridLineLayer.ActualWidth * TimelineLength.TotalMilliseconds));
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
            _offsetPercentageToCanvas = (_ScrollViewer.HorizontalOffset + _offsetAbsolutToScrollViewer) / _GridLineLayer.ActualWidth;

            //Update our grid
            _UpdateGridLineLayer();

            //Scroll to saved position and update layout for further functions
            _ScrollViewer.ScrollToHorizontalOffset(_offsetPercentageToCanvas * _GridLineLayer.ActualWidth - _offsetAbsolutToScrollViewer);
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
            _MouseTooltip.Time = TimeSpan.FromMilliseconds(_XPosition / _GridLineLayer.ActualWidth * TimelineLength.TotalMilliseconds);
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

            if (e.VerticalChange != 0)
                _GridLineTooltipLayer.UpdateScrolling(e.VerticalOffset);
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

            _UpdateGridLineLayer();
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
            _GridLineLayer = new Layer.GridLines();
            _GridLineLayer.Background = Brushes.DarkSlateGray;
            _GridLineLayer.TimelineLength = TimelineLength;

            //_DynamicTooltipCanvas = new Canvas();
            ////_DynamicTooltipCanvas.Background = Brushes.Red;
            //_DynamicTooltipCanvas.VerticalAlignment = VerticalAlignment.Top;
            //_DynamicTooltipCanvas.MinHeight = 20;

            _GridLineTooltipLayer = new Layer.GridLineTooltips(_GridLineLayer.GridLineParameters);

            //TODO: somehow implement this MouseTooltipLine to not flicker like hell, idk why it does
            _MouseTooltipLine = new Rectangle();
            _MouseTooltipLine.Width = 2;
            _MouseTooltipLine.Fill = Brushes.Red;
            _MouseTooltipLine.Visibility = Visibility.Hidden;
            //_LineGridCanvas.Children.Add(_MouseTooltipLine);
            //_LineGridCanvasChildrenOffset++;            

            _ScrollViewerContentWrapper = new Grid();            
            _ScrollViewerContentWrapper.Children.Add(_GridLineLayer);
            _ScrollViewerContentWrapper.Children.Add(_GridLineTooltipLayer);

            _LineGridOverlay = new Grid();
            RowDefinition r1 = new RowDefinition();
            r1.Height = new GridLength(20);
            RowDefinition r2 = new RowDefinition();
            r2.Height = new GridLength(1, GridUnitType.Star);
            _LineGridOverlay.RowDefinitions.Add(r1);
            _LineGridOverlay.RowDefinitions.Add(r2);

            TimelineLanes _TimelineLanes = new TimelineLanes();
            _TimelineLanes.Add();

            //Grid.SetRow(_DynamicTooltipCanvas, 0);
            Grid.SetRow(_TimelineLanes, 1);

            //_LineGridOverlay.Children.Add(_DynamicTooltipCanvas);
            _LineGridOverlay.Children.Add(_TimelineLanes);

            _ScrollViewerContentWrapper.Children.Add(_LineGridOverlay);

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

            //_ScrollViewer.ScrollChanged += _ScrollViewer_ScrollChanged1;
        }

        //private void _ScrollViewer_ScrollChanged1(object sender, ScrollChangedEventArgs e)
        //{
        //    if (e.VerticalChange != 0)
        //        _DynamicTooltipCanvas.MinHeight = 20 + e.VerticalOffset;
        //}
    }
}
