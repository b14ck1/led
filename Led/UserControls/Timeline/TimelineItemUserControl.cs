using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Led.UserControls.Timeline
{
    class TimelineItemUserControl : Grid
    {
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
        private ViewModels.EffectBaseVM _EffectBaseVM;
        /// <summary>
        /// Type of effect to display and to cast _EffectBaseVM to
        /// </summary>
        private EffectType _EffectType;

        private Border _Border;

        private Rectangle _ColorDisplay;

        private Label _Information;

        

        public TimelineItemUserControl(ViewModels.EffectBaseVM effectBaseVM)
        {
            _EffectBaseVM = effectBaseVM;
            
            Width = _MinWidth;
            MaxHeight = _Height;
            Height = _Height;
            
            _InitComponents();
        }

        private void _InitComponents()
        {
            _Border = new Border();
            _Border.Background = Brushes.LightGray;
            _Border.BorderBrush = Brushes.SlateGray;
            _Border.BorderThickness = new System.Windows.Thickness(1);
            _Border.CornerRadius = new System.Windows.CornerRadius(2);
            _Border.Padding = new System.Windows.Thickness(3);
            Children.Add(_Border);

            _ColorDisplay = new Rectangle();
            _ColorDisplay.Width = _ColorDisplaySize;
            _ColorDisplay.Height = _ColorDisplaySize;
            _ColorDisplay.Fill = Brushes.Blue;
            Children.Add(_ColorDisplay);
        }
    }
}
