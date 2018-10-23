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
        /// All leds in this group ordered in respect to the wiring of the physical leds
        /// </summary>
        [JsonProperty]
        public List<Point> Leds { get; set; }

        /// <summary>
        /// All relevant information to display this group
        /// </summary>
        [JsonProperty]
        public LedGroupViewProperty View { get; set; }

        public Point MaxLeds
        {
            get
            {
                int _maxGridX = View.LedGrid.GetLength(0);
                int _maxGridY = View.LedGrid.GetLength(1);

                Point _start = new Point(_maxGridX, _maxGridY);
                Point _end = new Point(0, 0);

                for (int x = 0; x < _maxGridX; x++)
                {
                    for (int y = 0; y < _maxGridY; y++)
                    {
                        if(View.LedGrid[x,y].Status)
                        {
                            if (x < _start.X)
                                _start.X = x;
                            if (y < _start.Y)
                                _start.Y = y;

                            if (x > _end.X)
                                _end.X = x;
                            if (y > _end.Y)
                                _end.Y = y;
                        }
                    }
                }

                return new Point(_end.X - _start.X + 1, _end.Y - _start.Y + 1);
            }
        }

        public LedGroup()
        {
            PositionInEntity = new Point();
            Leds = new List<Point>();
            View = new LedGroupViewProperty();
        }
    }
}
