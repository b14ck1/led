using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Media;

namespace Led.Model
{
    [JsonObject]
    public class LedChangeData : INPC
    {
        [JsonProperty]
        public List<Utility.LedModelID> LedIDs;

        [JsonProperty]
        public Color Color;

        [JsonProperty]
        public ushort EffectID;

        public LedChangeData(List<Utility.LedModelID> LedIDs, Color Color, ushort EffectID)
        {
            this.LedIDs = LedIDs;
            this.Color = Color;
            this.EffectID = EffectID;
        }
    }
}
