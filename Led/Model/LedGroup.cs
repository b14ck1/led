using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;

namespace Led.Model
{
    [JsonObject]
    public class LedGroup
    {
        /// <summary>
        /// Which bus
        /// </summary>
        [JsonProperty]
        public byte BusID { get; set; }

        /// <summary>
        /// Position on the bus (hard wiring)
        /// </summary>
        [JsonProperty]
        public byte PositionInBus { get; set; }

        /// <summary>
        /// Position in the global grid of the base entity
        /// </summary>
        [JsonProperty]
        public Point PositionInEntity { get; set; }

        /// <summary>
        /// All leds in this group in respect to the wiring of the physical leds
        /// </summary>
        [JsonProperty]
        public List<Point> Leds { get; set; }

        /// <summary>
        /// All relevant information to display this group
        /// </summary>
        [JsonProperty]
        public LedGroupViewProperty View { get; set; }

        public LedGroup()
        {
            PositionInEntity = new Point();
            Leds = new List<Point>();
            View = new LedGroupViewProperty();
        }
    }
}
