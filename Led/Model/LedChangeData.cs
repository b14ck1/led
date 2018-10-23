using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Media;

namespace Led.Model
{
    [JsonObject]
    public class LedChangeData
    {
        [JsonProperty]
        public Utility.LedModelID LedID { get; set; }

        [JsonProperty]
        public Color Color { get; set; }

        [JsonProperty]
        public ushort EffectID { get; set; }

        public LedChangeData(Utility.LedModelID ledID, Color color, ushort effectID)
        {
            LedID = ledID;
            Color = color;
            EffectID = effectID;
        }
    }
}
