using Newtonsoft.Json;
using System.Collections.Generic;

namespace Led.Model
{
    [JsonObject]
    public class Frame
    {
        [JsonProperty]
        public List<LedChangeData> LedChanges { get; set; }

        public Frame()
        {

        }
    }
}
