using System.Collections.Generic;
using System.Windows.Media;

namespace Led.Model.Effect
{
    class EffectGroup : EffectBase
    {
        public List<EffectBase> Effects { get; set; }

        public EffectGroup(ushort startFrame = 0, ushort endFrame = 0)
            : base(EffectType.Group, startFrame, endFrame)
        {
            Effects = new List<EffectBase>();
        }
    }
}