using Newtonsoft.Json;
using System.Collections.Generic;

namespace Led.Model
{
    [JsonObject]
    class Project : INPC
    {
        [JsonProperty]
        public string ProjectName;

        [JsonProperty]
        public byte FramesPerSecond;

        [JsonProperty]
        public AudioProperty AudioProperty;

        [JsonProperty]
        public List<LedEntity> LedEntities;

        public Project(string Name)
        {
            ProjectName = Name;
            LedEntities = new List<LedEntity>();
        }
    }
}
