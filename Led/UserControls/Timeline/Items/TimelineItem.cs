using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Led.UserControls.Timeline.Items
{
    class TimelineItem : Canvas
    {
        public Layer.GridLineParameters GridLineParameters;

        public event EventHandler OnPositionChanged;

        public static int ColorDisplaySize = 24;

        /// <summary>
        /// Minimal width of the whole item in pixels
        /// </summary>
        private static int _MinWidth = 30;

        /// <summary>
        /// Height of the whole item in pixels
        /// </summary>
        private static int _Height = 30;        

        /// <summary>
        /// Model with all Information
        /// </summary>
        private ITimelineItem _TimelineItem;

        private Border _Border;

        private Rectangle _ColorDisplay;

        private Point _LeftMouseButtonDownPosition;
        private bool _LeftMouseButtonDown;

        public TimeSpan StartTime
        {
            get => (TimeSpan)GetValue(StartTimeProperty);
            set { SetValue(StartTimeProperty, value); }
        }
        public static DependencyProperty StartTimeProperty =
            DependencyProperty.Register(nameof(StartTime), typeof(TimeSpan), typeof(TimelineItem),
                new UIPropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(_OnStartTimeChanged)));
        private static void _OnStartTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {            
            TimelineItem item = (sender as TimelineItem);
            SetLeft(item, item.StartTime.TotalMilliseconds * item.GridLineParameters.PixelsPerMillisecond);            

            item.OnPositionChanged?.Invoke(item, null);
        }

        public TimeSpan EndTime
        {
            get => (TimeSpan)GetValue(EndTimeProperty);
            set { SetValue(EndTimeProperty, value); }
        }
        public static DependencyProperty EndTimeProperty =
            DependencyProperty.Register(nameof(EndTime), typeof(TimeSpan), typeof(TimelineItem),
                new UIPropertyMetadata(TimeSpan.Zero, new PropertyChangedCallback(_OnEndTimeChanged)));
        private static void _OnEndTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TimelineItem item = (sender as TimelineItem);
            double _newWidth = (item.EndTime.TotalMilliseconds - item.StartTime.TotalMilliseconds) * item.GridLineParameters.PixelsPerMillisecond;

            if (_newWidth >= 0)
            {
                item.Width = _newWidth;
                item._Border.Width = _newWidth;
            }

            item.OnPositionChanged?.Invoke(item, null);
        }

        public TimelineItem(ITimelineItem timelineItem, Layer.GridLineParameters gridLineParameters)
        {
            _TimelineItem = timelineItem;
            GridLineParameters = gridLineParameters;
            
            SetBinding(StartTimeProperty, timelineItem.StartTime);
            SetBinding(EndTimeProperty, timelineItem.EndTime);

            MinWidth = _MinWidth;
            Width = _MinWidth;
            MaxHeight = _Height;
            Height = _Height;

            Background = Brushes.Transparent;

            _InitComponents();

            PreviewMouseLeftButtonDown += TimelineItem_PreviewMouseLeftButtonDown;
            PreviewMouseLeftButtonUp += TimelineItem_MouseLeftButtonUp;
            PreviewMouseMove += TimelineItem_PreviewMouseMove;
            MouseLeave += TimelineItem_MouseLeave;            
        }

        private void TimelineItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _LeftMouseButtonDown = false;
        }

        public void UpdatePosition()
        {
            if (GridLineParameters != null)
            {
                SetLeft(this, GridLineParameters.PixelsPerMillisecond * StartTime.TotalMilliseconds);
                Width = GridLineParameters.PixelsPerMillisecond * (EndTime.TotalMilliseconds - StartTime.TotalMilliseconds);
                _Border.Width = GridLineParameters.PixelsPerMillisecond * (EndTime.TotalMilliseconds - StartTime.TotalMilliseconds);
            }
        }

        private void TimelineItem_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(_LeftMouseButtonDown)
            {
                Point _newMousePosition = e.GetPosition(Parent as IInputElement);
                double _deltaX = _newMousePosition.X - _LeftMouseButtonDownPosition.X;
                double _millisecondsToMove = _deltaX / GridLineParameters.PixelsPerMillisecond;

                int _framesToMove = (int)(_millisecondsToMove / Defines.MillisecondsPerFrame);
                _millisecondsToMove = _framesToMove * Defines.MillisecondsPerFrame;
                if (_millisecondsToMove != 0)
                {
                    _LeftMouseButtonDownPosition.X = _LeftMouseButtonDownPosition.X + _millisecondsToMove * GridLineParameters.PixelsPerMillisecond;
                    StartTime = TimeSpan.FromMilliseconds(StartTime.TotalMilliseconds + _millisecondsToMove);
                }
            }
        }

        protected override System.Windows.Media.HitTestResult HitTestCore(System.Windows.Media.PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        private void TimelineItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Save our ButtonState to process a click event
            _LeftMouseButtonDown = true;

            //Save the initial mouse position to handle mouse moving
            _LeftMouseButtonDownPosition = e.GetPosition(Parent as IInputElement);

            //Prevent any child to handle the mouse event
            e.Handled = true;
        }


        private void TimelineItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_LeftMouseButtonDown)
            {
                _LeftMouseButtonDown = false;
                if (_TimelineItem != null)
                    _TimelineItem.OnSelected();
            }
            e.Handled = true;
        }

        private void _InitComponents()
        {
            _Border = new Border();
            _Border.Background = Brushes.LightGray;
            _Border.BorderBrush = Brushes.SlateGray;
            _Border.BorderThickness = new System.Windows.Thickness(1);
            _Border.CornerRadius = new System.Windows.CornerRadius(2);
            _Border.Padding = new System.Windows.Thickness(2);
            _Border.MinWidth = _MinWidth;
            Children.Add(_Border);

            _ColorDisplay = new Rectangle();
            _ColorDisplay.Width = ColorDisplaySize;
            _ColorDisplay.Height = ColorDisplaySize;
            _ColorDisplay.Fill = Brushes.Blue;
            _Border.Child = _ColorDisplay;
        }
    }
}