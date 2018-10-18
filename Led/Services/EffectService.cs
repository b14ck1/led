using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        private List<LedEntityBaseVM> _Entities;
        private LedEntityBaseVM _CurrentLedEntity;

        private long _LastTickedFrame;
        private long _LastPreviewedFrame;
        private long _LastRecievedFrame;

        private Utility.AccurateTimer _AccurateTimer;

        public System.Windows.Window MainWindow;

        public void Init(ObservableCollection<LedEntityBaseVM> entities)
        {
            _Entities.Clear();
            foreach(var x in entities)
            {
                _Entities.Add(x);
            }
        }

        public EffectService()
        {
            _Entities = new List<LedEntityBaseVM>();

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        private void _RenderEntity(LedEntityBaseVM ledEntity)
        {
            List<IEffectLogic> _Effects = new List<IEffectLogic>();
            ledEntity.LedEntity.Effects.ForEach(x => _Effects.Add(x as IEffectLogic));

            foreach (var Effect in _Effects)
            {
                if (Effect.Active)
                {
                    for (ushort i = Effect.StartFrame; i < Effect.EndFrame; i++)
                    {
                        ledEntity.LedEntity.Seconds[i / Defines.FramesPerSecond].Frames[i % Defines.FramesPerSecond].LedChanges.AddRange(Effect.LedChangeDatas);
                    }
                }
            }
        }

        private void _RenderAllEntities()
        {
            _Entities.ForEach(x => _RenderEntity(x));
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
                _StartTimer();
            }
        }

        private void _PlayPreview(ViewModels.EffectBaseVM effectBaseVM)
        {
            effectBaseVM.EffectBase.LedChangeDatas
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

        private void _SetViewChangeData(LedEntityBaseVM ledEntity, y)

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
        /// <param name="ledChangeDatas">New list of ledChangeDatas starting with the latest.</param>
        /// <returns></returns>
        private List<Model.LedChangeData> _CumulateLedChangeDatas(List<List<Model.LedChangeData>> ledChangeDatas)
        {
            List<Model.LedChangeData> _LedChangeDatas = new List<Model.LedChangeData>();
            List<Utility.LedModelID> _LedIDs = new List<Utility.LedModelID>();
            for (int i = 0; i < ledChangeDatas.Count; i++)
            {
                ledChangeDatas[i].ForEach(x => _LedIDs.AddRange(x.LedIDs));

                for (int j = 0; j < ledChangeDatas.Count - i; j++)
                {
                    ledChangeDatas[i + j].ForEach(x => x.LedIDs.RemoveAll(LedID => _LedIDs.Contains(LedID)));
                }

                _LedChangeDatas.AddRange(ledChangeDatas[i]);
            }

            return _LedChangeDatas;
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
