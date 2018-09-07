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

        public EffectFadeColor()
            : base(EffectType.Fade)
        {
            this.Color = Color;
        }
    }
}
