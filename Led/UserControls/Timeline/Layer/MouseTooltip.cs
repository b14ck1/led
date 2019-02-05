﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Led.UserControls.Timeline.Layer
{


    class MouseTooltip : Canvas
    {
        private Items.TimelineTimeTooltip _Tooltip;
        private Rectangle _TooltipLine;

        public MouseTooltip()
        {
            _TooltipLine = new Rectangle();
            _TooltipLine.Width = 2;
            _TooltipLine.Height = 200;
            _TooltipLine.Fill = Brushes.Red;
            Children.Add(_TooltipLine);

            _Tooltip = new Items.TimelineTimeTooltip(TimeSpan.FromMilliseconds(0));
            Children.Add(_Tooltip);

            Visibility = Visibility.Hidden;
        }

        public void Update(double xPosition, TimeSpan timeToShow)
        {
            if (_Tooltip.Time != timeToShow)
            {
                _Tooltip.Time = timeToShow;
                _Tooltip.UpdateLayout();
            }

            if (_Tooltip.XOffset != xPosition - _Tooltip.ActualWidth / 2)
            {
                _Tooltip.XOffset = xPosition - _Tooltip.ActualWidth / 2;
                SetLeft(_TooltipLine, xPosition - _TooltipLine.ActualWidth / 2);
            }
        }

        public void UpdateScrolling(double offset)
        {
            SetTop(_Tooltip, offset);
            _TooltipLine.Height = ActualHeight;
        }
    }
}