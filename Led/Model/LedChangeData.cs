using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Media;

namespace Led.Model
{
    [JsonObject]
    public class LedChangeData : INPC
    {
        [JsonProperty]
        public List<Utility.LedModelID> LedIDs { get; set; }

        [JsonProperty]
        public Color Color { get; set; }

        [JsonProperty]
        public ushort EffectID { get; set; }

        public LedChangeData(List<Utility.LedModelID> ledIDs, Color color, ushort effectID)
        {
            LedIDs = ledIDs;
            Color = color;
            EffectID = effectID;
        }
    }
}
