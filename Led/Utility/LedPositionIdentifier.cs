using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Utility
{
    public class LedPositionIdentifier
    {
        public Model.LedGroup LedGroup { get; }

        public LedModelID LedID { get; }

        public LedPositionIdentifier(LedModelID ledID, ViewModels.EffectBaseVM effectBaseVM)
        {
            LedID = ledID;
            LedGroup = App.Instance.EffectService.GetLedGroup(ledID, effectBaseVM);
        }
    }
}
