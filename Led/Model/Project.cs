using Newtonsoft.Json;
using System.Collections.Generic;

namespace Led.Model
{
    [JsonObject]
    public class Project : INPC
    {
        [JsonProperty]
        public string ProjectName { get; set; }

        [JsonProperty]
        public byte FramesPerSecond { get; }

        [JsonProperty]
        public AudioProperty AudioProperty { get; set; }

        [JsonProperty]
        public List<LedEntity> LedEntities { get; }

        [JsonProperty]
        public List<System.Windows.Media.Color> GlobalColors { get; }

        [JsonConstructor]
        private Project()
        {
            LedEntities = new List<LedEntity>();
            GlobalColors = new List<System.Windows.Media.Color>();
        }

        public Project(string projectName)
        {
            ProjectName = projectName;
            LedEntities = new List<LedEntity>();
            GlobalColors = new List<System.Windows.Media.Color>();
            for(int i = 0; i < Defines.ColorsPerEntity; i++)
            {
                GlobalColors.Add(System.Windows.Media.Colors.Transparent);
            }

            FramesPerSecond = Defines.FramesPerSecond;
        }        
    }
}