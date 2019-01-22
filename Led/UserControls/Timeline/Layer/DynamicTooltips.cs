using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Led.UserControls.Timeline.Layer
{
    class DynamicTooltips : Canvas
    {
        public int TooltipHeight
        {
            get => _TooltipHeight;
            set
            {
                if (_TooltipHeight != value)
                {
                    _TooltipHeight = value;
                }
            }
        }
        private int _TooltipHeight;

        public DynamicTooltips()
        {

        }

        public void Update(GridLineParameters gridLineParameters)
        {

        }
    }
}
