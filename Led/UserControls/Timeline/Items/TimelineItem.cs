using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Led.UserControls.Timeline.Items
{
    class TimelineItem : Canvas
    {
        public Layer.GridLineParameters GridLineParameters;

        /// <summary>
        /// Minimal width of the whole item in pixels
        /// </summary>
        private static int _MinWidth = 30;

        /// <summary>
        /// Height of the whole item in pixels
        /// </summary>
        private static int _Height = 30;

        private static int _ColorDisplaySize = 24;

        /// <summary>
        /// Model with all Information
        /// </summary>
        private ITimelineItem _TimelineItem;

        private Border _Border;

        private Rectangle _ColorDisplay;

        private Label _Information;

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
            Console.WriteLine("Changed Start Time to: " + item.StartTime);
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
            item.Width = (item.EndTime.TotalMilliseconds - item.StartTime.TotalMilliseconds) * item.GridLineParameters.PixelsPerMillisecond;
        }

        public TimelineItem(ITimelineItem timelineItem)
        {
            _TimelineItem = timelineItem;
            
            SetBinding(StartTimeProperty, timelineItem.StartTime);
            SetBinding(EndTimeProperty, timelineItem.EndTime);

            MinWidth = _MinWidth;
            Width = _MinWidth;
            MaxHeight = _Height;
            Height = _Height;

            _InitComponents();

            MouseLeftButtonUp += TimelineItemUserControl_MouseLeftButtonUp;

            Background = Brushes.Aquamarine;
        }

        public void UpdatePosition()
        {
            if (GridLineParameters != null)
            {                
                SetLeft(this, GridLineParameters.PixelsPerMillisecond * StartTime.TotalMilliseconds);
                Width = GridLineParameters.PixelsPerMillisecond * (StartTime.TotalMilliseconds - EndTime.TotalMilliseconds);
            }
        }

        private void TimelineItemUserControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(_TimelineItem != null)
                _TimelineItem.OnSelected();
        }

        private void _InitComponents()
        {
            _Border = new Border();
            _Border.Background = Brushes.LightGray;
            _Border.BorderBrush = Brushes.SlateGray;
            _Border.BorderThickness = new System.Windows.Thickness(1);
            _Border.CornerRadius = new System.Windows.CornerRadius(2);
            _Border.Padding = new System.Windows.Thickness(2);
            Children.Add(_Border);

            _ColorDisplay = new Rectangle();
            _ColorDisplay.Width = _ColorDisplaySize;
            _ColorDisplay.Height = _ColorDisplaySize;
            _ColorDisplay.Fill = Brushes.Blue;
            _Border.Child = _ColorDisplay;
        }
    }
}
