using Newtonsoft.Json;

namespace Led.Model
{
    [JsonObject]
    public class LedGridCell
    {
        [JsonProperty]
        public bool Status { get; set; }

        [JsonProperty]
        public LedViewArrowDirection Direction { get; set; } 

        public LedGridCell()
        {
            Status = true;
            Direction = LedViewArrowDirection.None;
        }
    }
}
