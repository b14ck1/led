using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Led.Model
{
    [JsonObject]
    class LedEntity : INPC
    {
        [JsonProperty]
        public string LedEntityName;

        [JsonProperty]
        public Dictionary<byte, LedBus> LedBuses;

        [JsonProperty]
        public List<Effect.EffectBase> Effects;

        [JsonProperty]
        public Dictionary<LedEntityView, ImageInfo> ImageInfos;

        public LedEntity()
        {
            LedBuses = new Dictionary<byte, LedBus>();            
            Effects = new List<Effect.EffectBase>();
            ImageInfos = new Dictionary<LedEntityView, ImageInfo>();
            ImageInfos.Add(LedEntityView.Front, new ImageInfo());
            ImageInfos.Add(LedEntityView.Back, new ImageInfo());
        }
    }
}
