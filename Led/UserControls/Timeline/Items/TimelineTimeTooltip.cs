using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace Led.UserControls.Timeline.Items
{
    class TimelineTimeTooltip : Grid
    {
        public TimeSpan Time
        {
            get => _Time;
            set
            {
                _Time = value;
                string res = value.Minutes.ToString().PadLeft(2, '0') + ":" + value.Seconds.ToString().PadLeft(2, '0') + ":" + value.Milliseconds.ToString().PadLeft(3, '0');
                _Label.Content = res;
            }
        }
        private TimeSpan _Time;

        public double XOffset
        {
            get => (RenderTransform as TranslateTransform).X;
            set => (RenderTransform as TranslateTransform).X = value;
        }

        private Label _Label;

        public TimelineTimeTooltip(TimeSpan time)
        {
            RenderTransform = new TranslateTransform();
            Background = Brushes.White;

            _Label = new Label();
            _Label.Content = "00:00:00";
            _Label.Margin = new Thickness(0);
            _Label.Padding = new Thickness(1);
            _Label.BorderBrush = Brushes.Aqua;
            _Label.BorderThickness = new Thickness(1);

            Time = time;

            Children.Add(_Label);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (double.IsPositiveInfinity(MaxHeight))
                MaxHeight = 20;
            _Label.FontSize = MaxHeight > 0 ? 0.6 * MaxHeight : 10;
            base.OnRender(dc);
        }
    }
}
