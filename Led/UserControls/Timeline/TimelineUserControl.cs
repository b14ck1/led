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
    class TimelineUserControl : Canvas, IParticipant
    {
        private IMediator _Mediator;
        /// <summary>
        /// Items to display
        /// </summary>
        private ObservableCollection<ViewModels.EffectBaseVM> _EffectBaseVMs;        
        /// <summary>
        /// When two items would overlap we would draw them on two different lanes.
        /// E.g.: this would bet set to two.
        /// </summary>
        private int _LanesToDraw;

        public bool ZoomLock
        {
            get => (bool)GetValue(ZoomLockProperty);
            set { SetValue(ZoomLockProperty, value); }
        }

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
        /// /// <param name="speed">Speed for scrolling from 0-1 in seconds.</param>
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

        //Gets called when we add or remove an effect
        private void _OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
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
