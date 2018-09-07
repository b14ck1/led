using System.Collections.Generic;
using System.Windows.Media;

namespace Led.Model.Effect
{
    public class EffectSetColor : EffectBase
    {
        public Color Color { get; set; }

        public EffectSetColor()
            : base(EffectType.SetColor)
        {
        }
    }
}
