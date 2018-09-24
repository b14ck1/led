using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Services
{
    class EffectService
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
    }
}
