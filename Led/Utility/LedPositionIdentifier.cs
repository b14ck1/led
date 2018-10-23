using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Utility
{
    public class LedPositionIdentifier
    {
        public System.Windows.Point LedPosition { get; }

        public System.Windows.Point GroupPosition { get; }

        public LedModelID LedID { get; }

        public LedPositionIdentifier(LedModelID ledID, ViewModels.EffectBaseVM effectBaseVM)
        {
            LedID = ledID;
            LedPosition = App.Instance.EffectService.GetLedPosition(LedID, effectBaseVM);
            GroupPosition = App.Instance.EffectService.GetGroupPosition(LedID, effectBaseVM);
        }
    }
}
