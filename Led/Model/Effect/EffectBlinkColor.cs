using System.Collections.Generic;
using System.Windows.Media;

namespace Led.Model.Effect
{
    public class EffectBlinkColor : EffectBase
    {
        public List<Color> Color { get; set; }
        public short BlinkFrames { get; set; }
        public short CurrColor { get; set; }

        public EffectBlinkColor()
            : base(EffectType.Blink)
        {
            Color = Color;
            BlinkFrames = BlinkFrames;
            CurrColor = 0;
        }
    }
}
