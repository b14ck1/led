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
            Color = Colors.Black;
        }

        public override List<LedChangeData> LedChangeDatas
        {
            get => new List<LedChangeData> { new LedChangeData(Leds, Color, ID) };
        }
    }
}
