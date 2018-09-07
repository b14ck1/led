using Newtonsoft.Json;
using System.Collections.Generic;

namespace Led.Model
{
    [JsonObject]
    class LedBus : INPC
    {
        [JsonProperty]
        public List<LedGroup> LedGroups;

        public LedBus()
        {
            LedGroups = new List<LedGroup>();
        }
    }
}
