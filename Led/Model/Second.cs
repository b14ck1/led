using Newtonsoft.Json;
using System.Collections.Generic;

namespace Led.Model
{
    [JsonObject]
    class Second
    {
        [JsonProperty]
        public Frame[] Frames { get; set; }

        [JsonProperty]        
        public List<LedChangeData> LedEntityStatus { get; set; }
        
        public Second()
        {

        }
    }
}
