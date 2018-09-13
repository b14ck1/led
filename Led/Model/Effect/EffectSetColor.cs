using System.Collections.Generic;
using System.Windows.Media;

namespace Led.Model.Effect
{
    public class EffectSetColor : EffectBase
    {
        public Color Color { get; set; }

        public EffectSetColor(ushort startFrame = 0, ushort endFrame = 0)
            : base(EffectType.SetColor, startFrame, endFrame)
        {
        }
    }
}
