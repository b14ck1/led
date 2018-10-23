using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using Led.ViewModels;

namespace Led.Services
{
    public class EffectService : Interfaces.IParticipant
    {
        /* The EffectService recieves the AudioTimer Message and updates all Entitys (AudioControlCurrentTick)
         * -> Maybe provide Async Update so the AudioControl won't get disturbed
         * -> Provides functionality to jump to a position in the timeline -> Check
         * -> Maybe just switch to a long instead of AudioControlCurrentTick in case of performance issues
         * 
         * The Entities must register themselves on construction
         * 
         * Implement different Updates
         *      Update the View in the App -> Check
         *      Update the Suit
         * -> Only the visible view will get updated
         * 
         * Provides functionality to test an effect in the view and the suit
         * -> Must implement a Timer (like AudioControl)
         * 
         * Provides functionality to pre render all effects and save them in the model (model/vm?)
         * */

        /*
         * Render MultiFrame Effects
         * 
         * Button to render all effects, not everytime we press play
         * EffectPreviewLedChangeData
         * 
         * _UpdateView ForceReload -> Force Load of Full Image
         * 
         * */

        private Services.MediatorService _Mediator;

        private List<LedEntityBaseVM> _LedEntityBaseVMs;
        private LedEntityBaseVM _CurrentLedEntity;

        private Model.AudioProperty _AudioProperty;

        private long _LastTickedFrame;
        private long _LastPreviewedFrame;
        private long _LastRecievedFrame;

        private Utility.AccurateTimer _AccurateTimer;

        public System.Windows.Window MainWindow;

        public void Init(ObservableCollection<LedEntityBaseVM> ledEntityBaseVMs)
        {
            _LedEntityBaseVMs.Clear();
            foreach (var x in ledEntityBaseVMs)
            {
                _LedEntityBaseVMs.Add(x);
            }
        }

        public List<Model.LedStatus> GetState(long frame, List<Utility.LedModelID> ledModelIDs, Model.Effect.EffectBase effectBase)
        {
            throw new NotImplementedException();
        }

        public Model.LedGroup GetLedGroup(Utility.LedModelID ledID, EffectBaseVM effectBaseVM)
        {
            foreach (var ledEntityBaseVM in _LedEntityBaseVMs)
            {
                if (ledEntityBaseVM.Effects.Contains(effectBaseVM))
                    return ledEntityBaseVM.LedEntity.LedBuses[ledID.BusID].LedGroups.Find(x => x.PositionInBus == ledID.PositionInBus);
            }

            return null;
        }

        public List<Point> CalculateRelativeLedPosition(List<Utility.LedModelID> ledModelIDs, EffectBaseVM effectBaseVM)
        {
            //Get all LedPositionIdentifiers to the corresponding LedIDs
            List<Utility.LedPositionIdentifier> LedPositions = new List<Utility.LedPositionIdentifier>();
            ledModelIDs.ForEach(x => LedPositions.Add(new Utility.LedPositionIdentifier(x, effectBaseVM)));

            //Map all Groups to their corresponding number of MaxLeds in x- and y-direction
            Dictionary<Model.LedGroup, Point> _maxGroupLeds = new Dictionary<Model.LedGroup, Point>();
            LedPositions.ForEach(x =>
            {
                if (!_maxGroupLeds.ContainsKey(x.LedGroup))
                    _maxGroupLeds.Add(x.LedGroup, x.LedGroup.MaxLeds);
            });

            //Map all Groups to their corresponding offset for their Leds
            Dictionary<Model.LedGroup, Point> _groupOffsets = new Dictionary<Model.LedGroup, Point>();
            _maxGroupLeds.Keys.ToList().ForEach(x =>
            {
                Point _offset = new Point(0, 0);

                //Identify all groups which are "before" the current group (before in x- and y-direction)
                List<Model.LedGroup> _ledGroupsBeforeX = new List<Model.LedGroup>();
                List<Model.LedGroup> _ledGroupsBeforeY = new List<Model.LedGroup>();
                _maxGroupLeds.Keys.Where(y => !y.Equals(x)).ToList().ForEach(z =>
                {
                    if (z.PositionInEntity.X < x.PositionInEntity.X && z.PositionInEntity.Y <= x.PositionInEntity.Y)
                        _ledGroupsBeforeX.Add(z);
                    if (z.PositionInEntity.X <= x.PositionInEntity.X && z.PositionInEntity.Y < x.PositionInEntity.Y)
                        _ledGroupsBeforeY.Add(z);
                });

                //Loop over all groups which are before in x-direction starting with the nearest stopping with x=0
                for (int i = (int)x.PositionInEntity.X - 1; i >= 0; i--)
                {
                    double _offsetX = 0;

                    //Check if there is more than one group with this x-value
                    //If yes gather the value which is larger
                    _ledGroupsBeforeX.Where(y => y.PositionInEntity.X == i).ToList().ForEach(z =>
                    {
                        if (_maxGroupLeds[z].X > _offsetX)
                            _offsetX = _maxGroupLeds[z].X;
                    });

                    //Add it to the x-offset
                    _offset.X += _offsetX;
                }

                //Loop over all groups which are before in y-direction starting with the nearest stopping with y=0
                for (int i = (int)x.PositionInEntity.Y - 1; i >= 0; i--)
                {
                    double _offsetY = 0;

                    //Check if there is more than one group with this y-value
                    //If yes gather the value which is larger
                    _ledGroupsBeforeX.Where(y => y.PositionInEntity.Y == i).ToList().ForEach(z =>
                    {
                        if (_maxGroupLeds[z].Y > _offsetY)
                            _offsetY = _maxGroupLeds[z].Y;
                    });

                    //Add it to the y-offset
                    _offset.Y += _offsetY;
                }

                //Save the calculatet offset
                _groupOffsets.Add(x, _offset);
            });

            //Iterate all LedIDs and add the corresponding offsets
            List<Point> res = new List<Point>();
            LedPositions.ForEach(x =>
            {
                Point _positionWithOffset = x.LedGroup.Leds[x.LedID.Led];
                _positionWithOffset.X += _groupOffsets[x.LedGroup].X;
                _positionWithOffset.Y += _groupOffsets[x.LedGroup].Y;
                res.Add(_positionWithOffset);
            });

            return res;
        }

        public EffectService()
        {
            _LedEntityBaseVMs = new List<LedEntityBaseVM>();

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }


        private void _InitAllSeconds()
        {
            if (_AudioProperty != null)
            {
                foreach (var ledEntityBaseVM in _LedEntityBaseVMs)
                {
                    _InitSeconds(ledEntityBaseVM);
                }
            }
        }

        private void _InitSeconds(LedEntityBaseVM ledEntityBaseVM)
        {
            ledEntityBaseVM.LedEntity.Seconds = new Model.Second[(int)_AudioProperty.Length.TotalSeconds];
            for (int i = 0; i < ledEntityBaseVM.LedEntity.Seconds.Length; i++)
            {
                ledEntityBaseVM.LedEntity.Seconds[i] = new Model.Second();

                for (int j = 0; j < Defines.FramesPerSecond; j++)
                {
                    ledEntityBaseVM.LedEntity.Seconds[i].Frames[j] = new Model.Frame();
                }
            }
        }

        private void _RenderAllEntities()
        {
            _LedEntityBaseVMs.ForEach(x => _RenderEntity(x));
        }

        private void _RenderEntity(LedEntityBaseVM ledEntity)
        {
            _InitSeconds(ledEntity);

            List<IEffectLogic> _Effects = new List<IEffectLogic>();
            ledEntity.LedEntity.Effects.ForEach(x => _Effects.Add(x as IEffectLogic));

            //Sort the List after StartFrame, if two Effects got the same StartFrame order the Fade-Effects Last
            _Effects.Sort(delegate (IEffectLogic x, IEffectLogic y)
            {
                if (x.StartFrame != y.StartFrame)
                    return x.StartFrame.CompareTo(y.StartFrame);
                else if (x.EffectType == EffectType.Fade && y.EffectType != EffectType.Fade)
                    return 1;
                else if (x.EffectType != EffectType.Fade && y.EffectType == EffectType.Fade)
                    return -1;
                else
                    return 0;

            });

            //Render every Effect one after another
            foreach (var Effect in _Effects)
            {
                if (Effect.Active)
                {
                    for (ushort i = Effect.StartFrame; i < Effect.EndFrame; i++)
                    {
                        List<Model.LedChangeData> ledChangeDatas = Effect.LedChangeDatas(i);
                        if (ledChangeDatas != null)
                            ledEntity.LedEntity.Seconds[i / Defines.FramesPerSecond].Frames[i % Defines.FramesPerSecond].LedChanges.AddRange(ledChangeDatas);
                        else
                            Debug.WriteLine(this + ": Something went wrong. Null Exception while rendering Entity.");
                    }
                }
            }

            //Save the FullImages (LedStatus) for every second
            //Start with writing every existing Led in the first Status

            ledEntity.LedEntity.Seconds[0].LedEntityStatus.Clear();
            ledEntity.LedEntity.AllLedIDs.ForEach(x => ledEntity.LedEntity.Seconds[0].LedEntityStatus.Add(new Model.LedChangeData(x, System.Windows.Media.Colors.Black, 0)));

            List<List<Model.LedChangeData>> ledChangeDatasForFirstFrame = new List<List<Model.LedChangeData>>();
            ledChangeDatasForFirstFrame.Add(ledEntity.LedEntity.Seconds[0].LedEntityStatus);

            for (int i = 0; i < ledEntity.LedEntity.Seconds.Length; i++)
            {
                List<List<Model.LedChangeData>> _ledChangeDatas = new List<List<Model.LedChangeData>>();
                if (i == 0)
                {
                    _ledChangeDatas.Add(ledEntity.LedEntity.Seconds[i].LedEntityStatus);
                    _ledChangeDatas.Add(ledEntity.LedEntity.Seconds[i].Frames[0].LedChanges);
                }
                else
                {
                    _ledChangeDatas.Add(ledEntity.LedEntity.Seconds[i - 1].LedEntityStatus);

                    for (int j = 0; j < Defines.FramesPerSecond; j++)
                    {
                        _ledChangeDatas.Add(ledEntity.LedEntity.Seconds[i - 1].Frames[j].LedChanges);
                    }
                }
                ledEntity.LedEntity.Seconds[i].LedEntityStatus = _CumulateLedChangeDatas(_ledChangeDatas);
            }
        }

        private void _PreviewEffect(ViewModels.EffectBaseVM effectBaseVM, bool stop)
        {
            if (stop)
            {
                _StopTimer();
                _UpdateView(_CurrentLedEntity, _LastTickedFrame);
            }
            else
            {
                _LastPreviewedFrame = effectBaseVM.StartFrame - 1;
                //_StartTimer();
            }
        }

        private void _PlayPreview(ViewModels.EffectBaseVM effectBaseVM)
        {
            //effectBaseVM.EffectBase.LedChangeDatas
        }

        private void _StartTimer(Action callback)
        {
            _AccurateTimer = new Utility.AccurateTimer(MainWindow, callback, 1000 / 40);
        }

        private void _StopTimer()
        {
            _AccurateTimer.Stop();
        }

        private void _SynchronizeTimerWithMusic()
        {
            if (_LastTickedFrame < _LastRecievedFrame - 1 || _LastTickedFrame > _LastRecievedFrame + 1)
            {
                _LastTickedFrame = _LastRecievedFrame;
                Debug.WriteLine("MusicTick: " + _LastRecievedFrame.ToString().PadLeft(10));
            }
        }

        private void _SynchronizeSuit()
        {

        }

        private void _SetViewChangeData(LedEntityBaseVM ledEntity)
        {

        }

        private void _UpdateView(LedEntityBaseVM ledEntity, long currentFrame)
        {
            if (ledEntity == null)
                return;

            //Get refs and compute the current second
            int currentSecond = (int)(currentFrame / Defines.FramesPerSecond);
            int currentFramesRespectiveCurrentSecond = (int)(currentFrame % Defines.FramesPerSecond);
            Model.Second[] seconds = ledEntity.LedEntity.Seconds;

            //Just some checking
            if (currentSecond >= seconds.Length)
            {
                Debug.WriteLine("UpdateFrame out of range. FrameValue: " + currentFrame + " SecondValue: " + currentSecond + "MaxSeconds: " + seconds.Length);
                return;
            }

            //When we are only one frame ahead of the ledEntity (normal) just execute the update
            if (currentFrame - 1 == ledEntity.CurrentFrame)
            {
                ledEntity.SetLedColor(seconds[currentSecond].Frames[currentFramesRespectiveCurrentSecond].LedChanges);
            }
            else
            {
                Debug.WriteLine("Out of sync. Updating to Frame: " + _LastTickedFrame + " from Frame: " + ledEntity.CurrentFrame);

                //When we aren't in the current Second, load the full image
                if (ledEntity.CurrentFrame / Defines.FramesPerSecond == currentSecond)
                {
                    ledEntity.SetLedColor(ledEntity.LedEntity.Seconds[currentSecond].LedEntityStatus);
                }

                //After this execute all FrameChanges till the current one
                List<List<Model.LedChangeData>> _LedChangeDatas = new List<List<Model.LedChangeData>>();
                for (int i = (int)(ledEntity.CurrentFrame % Defines.FramesPerSecond) + 1; i <= currentFramesRespectiveCurrentSecond; i++)
                {
                    _LedChangeDatas.Add(ledEntity.LedEntity.Seconds[currentSecond].Frames[i].LedChanges);
                }

                ledEntity.SetLedColor(_CumulateLedChangeDatas(_LedChangeDatas));
            }

            ledEntity.CurrentFrame = currentFrame;
        }

        private void _UpdateSuit(LedEntityBaseVM ledEntity, long currentFrame)
        {

        }

        /// <summary>
        /// Cumulates ledChangeDatas so that every LED is refreshed only once.
        /// </summary>
        /// <param name="ledChangeDatas">New list of ledChangeDatas starting with the earliest.</param>
        /// <returns></returns>
        private List<Model.LedChangeData> _CumulateLedChangeDatas(List<List<Model.LedChangeData>> ledChangeDatas)
        {
            ledChangeDatas.Reverse();

            List<Model.LedChangeData> _res = new List<Model.LedChangeData>();
            List<Utility.LedModelID> _ledIDs = new List<Utility.LedModelID>();

            for (int i = 0; i < ledChangeDatas.Count; i++)
            {
                _ledIDs.Clear();
                ledChangeDatas[i].ForEach(x => _ledIDs.Add(x.LedID));

                for (int j = 1; j < ledChangeDatas.Count - i; j++)
                {
                    //bool tmp;
                    //if (i == 19 && j == 21)
                    //    tmp = _ledIDs.Contains(new Utility.LedModelID(0, 0, 0));


                    ledChangeDatas[i + j].RemoveAll(x => _ledIDs.Contains(x.LedID));
                }

                _res.AddRange(ledChangeDatas[i]);
            }

            return _res;
        }

        private void _SendFrameDataToSuit(LedEntityBaseVM ledEntity)
        {

        }

        private void _PlayWithMusic()
        {
            Debug.WriteLine(_LastTickedFrame);
            _UpdateView(_CurrentLedEntity, _LastTickedFrame);
            _LastTickedFrame++;
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.LedEntitySelectButtonClicked:
                    _CurrentLedEntity = (sender as LedEntityBaseVM);
                    break;
                case MediatorMessages.AudioControlPlayPause:
                    _LastRecievedFrame = (data as MediatorMessageData.AudioControlPlayPauseData).CurrentFrame;
                    _LastTickedFrame = _LastRecievedFrame;
                    if ((data as MediatorMessageData.AudioControlPlayPauseData).Playing)
                        _StartTimer(_PlayWithMusic);
                    else
                        _StopTimer();
                    break;
                case MediatorMessages.AudioControlCurrentTick:
                    long _frameRecieved = (data as MediatorMessageData.AudioControlCurrentFrameData).CurrentFrame;
                    if (_LastRecievedFrame != _frameRecieved)
                    {
                        _LastRecievedFrame = _frameRecieved;
                        _SynchronizeTimerWithMusic();
                    }
                    break;
                case MediatorMessages.AudioProperty_NewAudio:
                    _AudioProperty = (data as MediatorMessageData.AudioProperty_NewAudio).AudioProperty;
                    _RenderAllEntities();
                    break;
                case MediatorMessages.EffectServiceRenderAll:
                    _RenderAllEntities();
                    break;
                case MediatorMessages.EffectServicePreview:
                    MediatorMessageData.EffectServicePreviewData effectServicePreviewData = (data as MediatorMessageData.EffectServicePreviewData);
                    _PreviewEffect(effectServicePreviewData.EffectBaseVM, effectServicePreviewData.Stop);
                    break;
                default:
                    break;
            }
        }
    }
}
