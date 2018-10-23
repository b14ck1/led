using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Led.Model
{
    [JsonObject]
    public class LedEntity
    {
        [JsonProperty]
        public string LedEntityName { get; set; }

        [JsonProperty]
        public Dictionary<byte, LedBus> LedBuses { get; set; }

        [JsonProperty]
        public List<Effect.EffectBase> Effects { get; set; }

        [JsonProperty]
        public Dictionary<LedEntityView, ImageInfo> ImageInfos { get; set; }

        public List<Utility.LedModelID> AllLedIDs
        {
            get
            {
                List<Utility.LedModelID> res = new List<Utility.LedModelID>();

                foreach(var _ledBus in  LedBuses.Values)
                {
                    foreach(var _ledGroup in _ledBus.LedGroups)
                    {
                        for (ushort i = 0; i < _ledGroup.Leds.Count; i++)                        
                        {
                            res.Add(new Utility.LedModelID(_ledGroup.BusID, _ledGroup.PositionInBus, i));
                        }
                    }
                }

                return res;
            }
        }

        public Second[] Seconds { get; set; }

        public LedEntity()
        {
            LedBuses = new Dictionary<byte, LedBus>();
            Effects = new List<Effect.EffectBase>();
            ImageInfos = new Dictionary<LedEntityView, ImageInfo>
            {
                { LedEntityView.Front, new ImageInfo() },
                { LedEntityView.Back, new ImageInfo() }
            };
        }
    }
}
