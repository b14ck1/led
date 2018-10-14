using Newtonsoft.Json;
using System.Collections.Generic;

namespace Led.Model
{
    [JsonObject]
    public class LedBus
    {
        [JsonProperty]
        public List<LedGroup> LedGroups { get; set; }

        public LedBus()
        {
            LedGroups = new List<LedGroup>();
        }
    }
}
