using Newtonsoft.Json;
using System.Collections.Generic;

namespace Led.Model
{
    [JsonObject]
    class LedBus : INPC
    {
        [JsonProperty]
        public List<LedGroup> LedGroups { get; set; }

        public LedBus()
        {
            LedGroups = new List<LedGroup>();
        }
    }
}
