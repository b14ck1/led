using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Led.ViewModels;

namespace Led.Services
{
    class EffectService : IEffectService
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

        private List<LedEntityBaseVM> _Entities;

        private LedEntityBaseVM _CurrentLedEntity;

        private byte _FramesPerSecond;

        public void RegisterEntity(LedEntityBaseVM ledEntity)
        {
            if (!_Entities.Contains(ledEntity))
                _Entities.Add(ledEntity);
        }

        public void Init(LedEntityBaseVM ledEntity, byte framesPerSecond)
        {
            _Entities.Clear();
            _CurrentLedEntity = ledEntity;
            _FramesPerSecond = framesPerSecond;
        }
        public EffectService()
        {
            _Entities = new List<LedEntityBaseVM>();
        }

        public void RenderObject(LedEntityBaseVM ledEntity)
        {
            List<Model.Effect.EffectBase> _Effects = ledEntity.LedEntity.Effects;

            foreach (var Effect in _Effects)
            {
                if (Effect.Active)
                {

                }
            }

            for (int i = 0; i < Data.Seconds.Count; i++)
            {
                for (int j = 0; j < Data.Seconds[i].Frames.Count; j++)
                {
                    if (Data.Seconds[i].Frames[j].Functions.Count > 0)
                    {
                        for (int k = 0; k < Data.Seconds[i].Frames[j].Functions.Count; k++)
                        {
                            temp.Add(Data.Seconds[i].Frames[j].Functions[k]);
                        }
                    }

                    foreach (var func in temp)
                    {
                        if (func as Util.IEffectLogic != null)
                            Data.Seconds[i].Frames[j].LedChanges.AddRange((func as Util.IEffectLogic).Calc(Logic, frame));
                        else if (func as List<Util.IEffectLogic> != null)
                            Data.Seconds[i].Frames[j].LedChanges.AddRange(Logic.OrderByPriority(func as List<Util.IEffectLogic>, frame));
                        //else
                        //LogDatShit  
                    }

                    frame++;
                }
            }
        }

        private void _UpdateView(LedEntityBaseVM ledEntity, long currentFrame)
        {
            //Get refs and compute the current second
            int currentSecond = (int)(currentFrame / _FramesPerSecond);
            int currentFramesRespectiveCurrentSecond = (int)(currentFrame % _FramesPerSecond);
            List<Model.Second> seconds = ledEntity.LedEntity.Seconds;

            //Just some checking
            if (currentSecond >= seconds.Count)
            {
                Console.WriteLine("UpdateFrame out of range. FrameValue: " + currentFrame + " SecondValue: " + currentSecond + "MaxSeconds: " + seconds.Count);
                return;
            }

            //When we are only one frame ahead of the ledEntity (normal) just execute the update
            if (currentFrame - 1 == ledEntity.CurrentFrame)
            {
                ledEntity.SetLedColor(seconds[currentSecond].Frames[currentFramesRespectiveCurrentSecond].LedChanges);
            }
            else
            {
                //When we aren't in the current Second, load the full image
                if (ledEntity.CurrentFrame / _FramesPerSecond == currentSecond)
                {
                    ledEntity.SetLedColor(ledEntity.LedEntity.Seconds[currentSecond].LedEntityStatus);
                }

                //After this execute all FrameChanges till the current one
                List<List<Model.LedChangeData>> _LedChangeDatas = new List<List<Model.LedChangeData>>();
                for (int i = (int)(ledEntity.CurrentFrame % _FramesPerSecond) + 1; i <= currentFramesRespectiveCurrentSecond; i++)
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
    }
}
