using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Led.Model.Effect
{
    [JsonObject]
    public class EffectFadeColor : EffectBase
    {
        public List<Color> Color { get; set; }

        public EffectFadeColor(ushort startFrame = 0, ushort endFrame = 0)
            : base(EffectType.Fade, startFrame, endFrame)
        {
            this.Color = Color;
        }
    }
}
