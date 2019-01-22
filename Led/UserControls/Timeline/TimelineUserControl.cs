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

        /// <summary>
        /// One main grid to rule them all.
        /// Divides the area into two vertical sections.
        /// |   StaticTooltips  | ZoomSlider |
        /// |    ScrollViewer   | ZoomSlider |
        /// </summary>
        private Grid _MainGrid;

        /// <summary>
        /// Holds the LineGridCanvas and dynamic Tooltips.
        /// </summary>
        private ScrollViewer _ScrollViewer;
        private Grid _ScrollViewerContentWrapper;

        private Layer.GridLines _GridLineLayer;
        private Layer.GridLineTooltips _GridLineTooltipLayer;
        private Layer.MouseTooltip _MouseTooltipLayer;

        /// <summary>
        /// ZoomSlider to zoom the timeline.
        /// </summary>
        private Slider _ZoomSlider;
        
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
            _GridLineLayer.MouseWheel += _GridLineLayer_MouseWheel;
            _GridLineLayer.MouseMove += _GridLineLayer_MouseMove;
            _GridLineLayer.MouseEnter += _GridLineLayer_MouseEnter;
            _GridLineLayer.MouseLeave += _GridLineLayer_MouseLeave;
            _ScrollViewer.ScrollChanged += _ScrollViewer_ScrollChanged;

            _Items = new List<TimelineItemUserControl>();
            _Items.Add(new TimelineItemUserControl(null));
            Children.Add(_Items[0]);
        }

        private void _GridLineLayer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Zoom relative to where our mouse is pointed
            _IsZoomWithMouseWheel_OnLineGrid = true;
            _MousePosition = e.GetPosition(sender as IInputElement);

            _ZoomSlider.Value += e.Delta * _ZoomSlider.TickFrequency / 240;

            _IsZoomWithMouseWheel_OnLineGrid = false;
            e.Handled = true;
        }

        private void _GridLineLayer_MouseMove(object sender, MouseEventArgs e)
        {
            double _xPosition = e.GetPosition(sender as IInputElement).X;
            TimeSpan _mouseTooltipTime = TimeSpan.FromMilliseconds(_xPosition / _GridLineLayer.ActualWidth * TimelineLength.TotalMilliseconds);
            _MouseTooltipLayer.Update(_xPosition - _ScrollViewer.HorizontalOffset, _mouseTooltipTime);
        }

        private void _GridLineLayer_MouseEnter(object sender, MouseEventArgs e)
        {
            _MouseTooltipLayer.Visibility = Visibility.Visible;
        }

        private void _GridLineLayer_MouseLeave(object sender, MouseEventArgs e)
        {
            _MouseTooltipLayer.Visibility = Visibility.Hidden;
        }

        private void _UpdateGridLineLayer()
        {
            _GridLineLayer.Update(_ZoomSlider.Value * (ActualWidth - _MainGrid.ColumnDefinitions[1].ActualWidth), ActualHeight);            
        }

        private void _UpdateGridLineTooltipLayer()
        {
            _GridLineTooltipLayer.Update();
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

            //_UpdateStaticTooltips();
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
        
        private void _ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
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

            _UpdateGridLineLayer();
            _UpdateGridLineTooltipLayer();
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
            //Layers
            _GridLineLayer = new Layer.GridLines
            {
                Background = Brushes.DarkSlateGray,
                TimelineLength = TimelineLength
            };

            _GridLineTooltipLayer = new Layer.GridLineTooltips(_GridLineLayer.GridLineParameters);

            _MouseTooltipLayer = new Layer.MouseTooltip();            

            TimelineLanes _TimelineLanes = new TimelineLanes();
            _TimelineLanes.Add();

            //Wrapper
            _ScrollViewerContentWrapper = new Grid();
            _ScrollViewerContentWrapper.Children.Add(_GridLineLayer);
            _ScrollViewerContentWrapper.Children.Add(_GridLineTooltipLayer);

            //ScrollViewer
            _ScrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
                Content = _ScrollViewerContentWrapper
            };

            //ZoomSlider
            _ZoomSlider = new Slider
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(7, 5, 7, 5),
                Minimum = 1,
                Maximum = 10,
                TickFrequency = 0.5
            };
            _ZoomSlider.ValueChanged += _ZoomSlider_ValueChanged;
            Grid.SetColumn(_ZoomSlider, 1);

            //MainGrid with 2 columns
            _MainGrid = new Grid();
            ColumnDefinition c1 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
            ColumnDefinition c2 = new ColumnDefinition { Width = GridLength.Auto };            
            _MainGrid.ColumnDefinitions.Add(c1);
            _MainGrid.ColumnDefinitions.Add(c2);

            _MainGrid.Children.Add(_ScrollViewer);
            _MainGrid.Children.Add(_MouseTooltipLayer);
            _MainGrid.Children.Add(_ZoomSlider);

            Children.Add(_MainGrid);
        }
    }
}
