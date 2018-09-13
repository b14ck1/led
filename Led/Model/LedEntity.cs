﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Led.Model
{
    [JsonObject]
    class LedEntity : INPC
    {
        [JsonProperty]
        public string LedEntityName { get; set; }

        [JsonProperty]
        public Dictionary<byte, LedBus> LedBuses { get; set; }

        [JsonProperty]
        public List<Effect.EffectBase> Effects { get; set; }

        [JsonProperty]
        public Dictionary<LedEntityView, ImageInfo> ImageInfos { get; set; }

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