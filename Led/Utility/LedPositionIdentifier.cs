using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Utility
{
    public class LedPositionIdentifier
    {
        public System.Drawing.Point LedPosition { get; }

        public System.Drawing.Point GroupPosition { get; }

        public LedModelID LedID { get; }

        public LedPositionIdentifier(LedModelID ledID, Model.Effect.EffectBase effectBase)
        {
            LedID = ledID;
            LedPosition = App.Instance.EffectService.GetLedPosition(LedID, effectBase);
            GroupPosition = App.Instance.EffectService.GetGroupPosition(LedID, effectBase);
        }
    }
}
