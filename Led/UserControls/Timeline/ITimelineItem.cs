using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Led.UserControls.Timeline
{
    interface ITimelineItem
    {
        Binding StartTime { get; }

        Binding EndTime { get; }

        /// <summary>
        /// Get's called when the user clicks on this TimelineObject
        /// </summary>
        void OnSelected();
    }
}
