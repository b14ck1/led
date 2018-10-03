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
         * -> Provides functionality to jump to a position in the timeline
         * -> Maybe just switch to a long instead of AudioControlCurrentTick in case of performance issues
         * 
         * The Entities must register themselves on construction
         * 
         * Implement different Updates
         *      Update the View in the App
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

        private void _UpdateView(LedEntityBaseVM ledEntity, long currentFrame)
        {
            int currentSecond = (int)(currentFrame / _FramesPerSecond);
            List<Model.Second> seconds = ledEntity.LedEntity.Seconds;

            if (currentSecond >= seconds.Count)
            {
                Console.WriteLine("UpdateFrame out of range. FrameValue: " + currentFrame + " SecondValue: " + currentSecond + "MaxSeconds: " + seconds.Count);
                return;
            }

            if (currentFrame - 1 == ledEntity.CurrentFrame)
            {

            }
        }

        private void _UpdateSuit(LedEntityBaseVM ledEntity, long currentFrame)
        {

        }

        private void _LoadFrameDataIntoView(LedEntityBaseVM ledEntity, List<Model.LedChangeData> ledChangeData)
        {
            ledEntity.SetLedColor(ledChangeData);
        }

        private void _LoadFrameDataIntoView(LedEntityBaseVM ledEntity, List<List<Model.LedChangeData>> ledChangeData)
        {
            
        }

        private void _SendFrameDataToSuit(LedEntityBaseVM ledEntity)
        {

        }
    }
}
