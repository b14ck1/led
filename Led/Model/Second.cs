using Newtonsoft.Json;
using System.Collections.Generic;

namespace Led.Model
{
    [JsonObject]
    public class Second
    {
        /// <summary>
        /// Represents the state changed of each Led in each Frame of this Second.
        /// </summary>
        [JsonProperty]
        public Frame[] Frames { get; set; }

        /// <summary>
        /// Represents the state of all Leds at the beginning of this Second.
        /// </summary>
        [JsonProperty]        
        public List<LedChangeData> LedEntityStatus { get; set; }
        
        public Second()
        {
            Frames = new Frame[Defines.FramesPerSecond];
            LedEntityStatus = new List<LedChangeData>();
        }
    }
}
