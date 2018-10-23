using Newtonsoft.Json;
using System.Windows.Media;

namespace Led.Model
{
    [JsonObject]
    public class LedStatus : INPC
    {
        [JsonProperty]
        public Utility.LedModelID LedModelID { get; set; }

        [JsonProperty]
        public Color Color { get; set; }

        public LedStatus(Utility.LedModelID ledModelID, Color color)
        {
            LedModelID = ledModelID;
            Color = color;
        }
    }
}
