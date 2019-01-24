using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Led.UserControls.Timeline.Layer
{
    class ObjectLanes : Grid, Interfaces.IParticipant
    {
        private Interfaces.IMediator _Mediator;

        public int LaneHeight { get; private set; }      

        //Need to change this to ITimelineItem but no time right now
        private ObservableCollection<ViewModels.EffectBaseVM> _TimelineObjects;
        private Dictionary<ITimelineItem, Items.TimelineItem> _TimelineItems;
        private Dictionary<ITimelineItem, Canvas> _TimelineLanes;

        private GridLineParameters _GridLangeParameters;

        public ObjectLanes(int laneHeight, int tooltipHeight)
        {
            _TimelineObjects = new ObservableCollection<ViewModels.EffectBaseVM>();
            _TimelineItems = new Dictionary<ITimelineItem, Items.TimelineItem>();
            _TimelineLanes = new Dictionary<ITimelineItem, Canvas>();

            LaneHeight = laneHeight;

            RowDefinition r1 = new RowDefinition { Height = new System.Windows.GridLength(tooltipHeight) };
            RowDefinitions.Add(r1);

            _AddLane();

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        public void Update(GridLineParameters gridLineParameters)
        {
            _GridLangeParameters = gridLineParameters;
            _TimelineItems.Values.ToList().ForEach(item => {
                item.GridLineParameters = gridLineParameters;
                item.UpdatePosition();
            });
        }

        private void _AddLane()
        {
            RowDefinition r1 = new RowDefinition { Height = new System.Windows.GridLength(LaneHeight) };
            RowDefinitions.Add(r1);

            Canvas c = new Canvas { Height = LaneHeight, Background = Brushes.Red };
            SetRow(c, RowDefinitions.Count - 1);
            Children.Add(c);
        }

        private void _RemoveLastLane()
        {
            if (RowDefinitions.Count > 1)
            {
                Children.RemoveAt(Children.Count - 1);
                RowDefinitions.RemoveAt(RowDefinitions.Count - 1);
            }
        }

        private void _TimelineObjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var o in e.NewItems)
                    {
                        _TimelineItems.Add(o as ITimelineItem, new Items.TimelineItem(o as ITimelineItem));
                        _TimelineItems[o as ITimelineItem].GridLineParameters = _GridLangeParameters;
                        ((Canvas)Children[0]).Children.Add(_TimelineItems[o as ITimelineItem]);                        
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:                    
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    _TimelineItems.Clear();
                    break;
                default:
                    break;
            }
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.TimeLineCollectionChanged:
                    //Clear existing items
                    //_TimelineItems.Clear();
                    //if (Children.Count > 1)
                    //    Children.RemoveRange(1, Children.Count - 1);
                    //_TimelineLanes.Clear();
                    
                    _TimelineObjects = (data as MediatorMessageData.TimeLineCollectionChangedData).Effects;

                    foreach (ITimelineItem o in _TimelineObjects) { _TimelineItems.Add(o, new Items.TimelineItem(o as ITimelineItem)); }
                    _TimelineObjects.CollectionChanged += _TimelineObjects_CollectionChanged;
                    break;
            }
        }
    }
}
