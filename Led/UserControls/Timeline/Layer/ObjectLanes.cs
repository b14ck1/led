using Led.UserControls.Timeline.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
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

        private GridLineParameters _GridLineParameters;

        public ObjectLanes(int laneHeight, int tooltipHeight)
        {
            _TimelineObjects = new ObservableCollection<ViewModels.EffectBaseVM>();
            _TimelineItems = new Dictionary<ITimelineItem, Items.TimelineItem>();

            LaneHeight = laneHeight;

            RowDefinition r1 = new RowDefinition { Height = new System.Windows.GridLength(tooltipHeight) };
            RowDefinitions.Add(r1);

            _AddLane();

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        public void Update(GridLineParameters gridLineParameters)
        {
            _GridLineParameters = gridLineParameters;
            _TimelineItems.Values.ToList().ForEach(item => {
                item.GridLineParameters = gridLineParameters;
                item.UpdatePosition();
            });
        }

        private void _UpdateLanes()
        {
            //At first iterate through all children, which represent the lanes
            for (int i = 0; i < Children.Count; i++)
            {
                //In each lane look for overlapping TimelineItems
                Canvas _lane = Children[i] as Canvas;
                for (int j = 1; j < _lane.Children.Count; j++)
                {
                    TimelineItem _first = _lane.Children[j - 1] as TimelineItem;
                    TimelineItem _second = _lane.Children[j] as TimelineItem;

                    //If something is overlapping
                    if (_DoItemsOverlap(_first, _second))
                    {
                        //Create a new lane if necessary
                        if (i + 1 >= Children.Count)
                            _AddLane();

                        //Always move the item which is more to the end of the timeline
                        TimelineItem _itemToMove;
                        if (_second.StartTime < _first.StartTime)
                            _itemToMove = _first;
                        else
                            _itemToMove = _second;

                        _lane.Children.Remove(_itemToMove);
                        (Children[i + 1] as Canvas).Children.Add(_itemToMove);
                    }
                }

                //Also look for free space in the lanes above our current lane
                if (i > 0)
                {
                    Canvas _aboveLane;
                    Canvas _currentLane = _lane;
                    //Iterate over all TimelineItems
                    for (int j = 0; j < _currentLane.Children.Count; j++)
                    {
                        TimelineItem _item = _lane.Children[j] as TimelineItem;
                        //Iterate over all aboveLanes
                        for (int k = 0; k < i; k++)
                        {
                            bool _isFreeSpace = true;
                            _aboveLane = Children[k] as Canvas;
                            //Iterate over all TimelineItems in the aboveLane
                            for (int l = 0; l < _aboveLane.Children.Count; l++)
                            {
                                TimelineItem _aboveItem = _aboveLane.Children[l] as TimelineItem;
                                _isFreeSpace = !_DoItemsOverlap(_item, _aboveItem) && _isFreeSpace;
                            }

                            if (_isFreeSpace)
                            {
                                _lane.Children.Remove(_item);
                                _aboveLane.Children.Add(_item);
                                break;
                            }
                        }
                    }
                }
            }

            //Iterate one last time to check if there are lanes left which can be savely deleted
            for (int i = Children.Count - 1; i >= 1; i--)
            {
                if ((Children[i] as Canvas).Children.Count == 0)
                    _RemoveLastLane();
                else
                    break;
            }
        }

        private void ObjectLanes_OnPositionChanged(object sender, EventArgs e)
        {
            _UpdateLanes();
        }

        private bool _DoItemsOverlap(TimelineItem first, TimelineItem second)
        {
            //Make sure we check the visual representation for overlappings (min size of TimelineItems)
            TimeSpan _firstEndTime = TimeSpan.FromMilliseconds(first.StartTime.TotalMilliseconds + first.ActualWidth / _GridLineParameters.PixelsPerMillisecond);
            TimeSpan _secondEndTime = TimeSpan.FromMilliseconds(second.StartTime.TotalMilliseconds + second.ActualWidth / _GridLineParameters.PixelsPerMillisecond);

            bool _isStartOverlapping = second.StartTime <= _firstEndTime && second.StartTime >= first.StartTime;
            bool _isWholeOverlapping = second.StartTime <= first.StartTime && _secondEndTime >= _firstEndTime;
            bool _isEndOverlapping = _secondEndTime >= first.StartTime && _secondEndTime <= _firstEndTime;

            return _isStartOverlapping || _isWholeOverlapping || _isEndOverlapping;
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
                        _TimelineItems[o as ITimelineItem].GridLineParameters = _GridLineParameters;
                        _TimelineItems[o as ITimelineItem].OnPositionChanged += ObjectLanes_OnPositionChanged;
                        ((Canvas)Children[0]).Children.Add(_TimelineItems[o as ITimelineItem]);
                        _UpdateLanes();
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
                    _UpdateLanes();
                    break;
            }
        }
    }
}
