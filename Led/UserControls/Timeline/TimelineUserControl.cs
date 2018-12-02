using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Led.Interfaces;

namespace Led.UserControls.Timeline
{
    internal enum TimelineViewLevel
    {
        Minutes,
        Seconds,
        Frames
    }

    class TimelineUserControl : Canvas, IParticipant
    {
        private IMediator _Mediator;
        private TimelineViewLevel _TimelineViewLevel;

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

        /// <summary>
        /// This canvas holds holds the background and all grid lines.
        /// </summary>
        private Canvas _GridCanvas;

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
                new UIPropertyMetadata(0, new PropertyChangedCallback(_OnTimelineLengthChanged)));

        private static void _OnTimelineLengthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>Gets or sets the background color of the timeline.</summary>
        /// <value>Background color of the GridCanvas which represents the background of the timeline.</value>
        /// <remarks><param>Default Brushes.DarkGray.</param></remarks>
        public Brush BackgroundColor
        {
            get => (Brush)GetValue(BackgroundColorProperty);
            set { SetValue(BackgroundColorProperty, value); }
        }
        /// <dpdoc />
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Brush), typeof(TimelineUserControl),
                new UIPropertyMetadata(Brushes.DarkGray, new PropertyChangedCallback(_OnBackgroundColorChanged)));

        private static void _OnBackgroundColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TimelineItemUserControl).Background = (sender as TimelineUserControl).BackgroundColor;
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
            _EffectBaseVMs.CollectionChanged += _OnCollectionChanged;
            _LanesToDraw = 1;

            _Mediator = App.Instance.MediatorService;
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
        }

        private void _RenderGrid(Canvas grid)
        {

        }

        private void _SendMessage(MediatorMessages message, object data)
        {

        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            throw new NotImplementedException();
        }
    }
}
