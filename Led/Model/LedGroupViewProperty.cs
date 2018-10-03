using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;

namespace Led.Model
{
    /// <summary>
    /// Saves all relevant information to display a led group
    /// </summary>
    [JsonObject]
    public class LedGroupViewProperty : INPC
    {
        /// <summary>
        /// On which view (side) is this group located
        /// </summary>
        [JsonProperty]
        public LedEntityView View { get; set; }

        /// <summary>
        /// On which grid position does the wiring start
        /// </summary>
        [JsonProperty]
        public Point StartPositionWiring { get; set; }

        /// <summary>
        /// All information about the grid. Led y/n, arrow y/n.
        /// </summary>
        [JsonProperty]
        public LedGridCell[,] LedGrid { get; set; }

        /// <summary>
        /// Start position of the group with respect to the image of the base entity
        /// </summary>
        [JsonProperty]
        public Point StartPositionOnImage { get; set; }

        /// <summary>
        /// Size of the group with respect to the image of the base entity
        /// </summary>
        [JsonProperty]
        public Size SizeOnImage { get; set; }

        /// <summary>
        /// When the first led isn't in the first row/column of the grid
        /// </summary>
        [JsonProperty]
        public Point GridLedStartOffset { get; set; }

        public LedGroupViewProperty()
        {
            StartPositionWiring = new Point();
            LedGrid = new LedGridCell[0, 0];
            StartPositionOnImage = new Point();
            SizeOnImage = new Size();
            GridLedStartOffset = new Point();
        }
    }
}
