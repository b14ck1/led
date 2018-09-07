using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;

namespace Led.Model
{
    [JsonObject]
    class LedGroup : INPC
    {
        /// <summary>
        /// Which bus
        /// </summary>
        [JsonProperty]
        public byte BusID;

        /// <summary>
        /// Position on the bus (hard wiring)
        /// </summary>
        [JsonProperty]
        public byte PositionInBus;

        /// <summary>
        /// Position in the global grid of the base entity
        /// </summary>
        [JsonProperty]
        public Point PositionInEntity;

        /// <summary>
        /// All leds in this group in respect to the wiring of the physical leds
        /// </summary>
        [JsonProperty]
        public List<Point> Leds;

        /// <summary>
        /// All relevant information to display this group
        /// </summary>
        [JsonProperty]
        public LedGroupViewProperty View;

        public LedGroup()
        {
            PositionInEntity = new Point();
            Leds = new List<Point>();
            View = new LedGroupViewProperty();
        }
    }
}
