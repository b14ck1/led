using System.Collections.Generic;
using System.Windows.Media;

namespace Led.Model.Effect
{
    class EffectGroup : EffectBase
    {
        public List<EffectBase> Effects { get; set; }

        public EffectGroup()
            : base(EffectType.Group)
        {
            Effects = new List<EffectBase>();
        }
    }
}
