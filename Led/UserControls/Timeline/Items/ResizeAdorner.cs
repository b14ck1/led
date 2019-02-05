using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Led.UserControls.Timeline.Items
{
    class ResizeAdorner : Adorner
    {
        SolidColorBrush _RenderBrush;
        
        public ResizeAdorner(UIElement uiElement)
            : base(uiElement)
        {
            _RenderBrush = new SolidColorBrush(Colors.LimeGreen);
            _RenderBrush.Opacity = 0.5;            
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            drawingContext.DrawRectangle(_RenderBrush, null, new Rect(0, 0, 5, ActualHeight));
            drawingContext.DrawRectangle(_RenderBrush, null, new Rect(ActualWidth - 5, 0, 5, ActualHeight));
        }
    }
}