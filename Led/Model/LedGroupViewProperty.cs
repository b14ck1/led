using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;

namespace Led.Model
{
    /// <summary>
    /// Saves all relevant information to display a led group
    /// </summary>
    [JsonObject]
    class LedGroupViewProperty : INPC
    {
        /// <summary>
        /// On which view (side) is this group located
        /// </summary>
        [JsonProperty]
        public LedEntityView View;

        /// <summary>
        /// On which grid position does the wiring start
        /// </summary>
        [JsonProperty]
        public Point StartPositionWiring;

        /// <summary>
        /// All information about the grid. Led y/n, arrow y/n.
        /// </summary>
        [JsonProperty]
        public LedGridCell[,] LedGrid;

        /// <summary>
        /// Start position of the group with respect to the image of the base entity
        /// </summary>
        [JsonProperty]
        public Point StartPositionOnImage;

        /// <summary>
        /// Size of the group with respect to the image of the base entity
        /// </summary>
        [JsonProperty]
        public Size SizeOnImage;

        /// <summary>
        /// When the first led isn't in the first row/column of the grid
        /// </summary>
        [JsonProperty]
        public Point GridLedStartOffset;
        
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
