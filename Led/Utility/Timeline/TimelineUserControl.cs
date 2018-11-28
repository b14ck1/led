using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Led.Utility.Timeline
{
    class TimelineUserControl : Canvas
    {
        private ObservableCollection<ViewModels.EffectBaseVM> _EffectBaseVMs;
        
        public TimelineUserControl()
        {
            
        }
    }
}
