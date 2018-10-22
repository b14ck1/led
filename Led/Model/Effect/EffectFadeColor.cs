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
        [JsonProperty]
        public bool SetColorAtBeginning { get; set; }

        [JsonProperty]
        public bool Repeat { get; set; }

        [JsonProperty]
        public int FramesForOneRepetition { get; set; }

        private int _FadeFrames { get => Dauer / Colors.Count; }

        private bool GotLedState { get; set; }

        private List<Model.LedStatus> _LedStatuses { get; set; }

        public EffectFadeColor(ushort startFrame = 0, ushort endFrame = 0)
            : base(EffectType.Fade, startFrame, endFrame)
        {
            Colors = new List<Color>()
            {
                System.Windows.Media.Colors.Black,
                System.Windows.Media.Colors.Black
            };

            Repeat = false;
            FramesForOneRepetition = Dauer;
        }

        private Color _Fade(long fadeStart, long fadeEnd, long fadeSearched, Color startColor, Color endColor)
        {
            long _fadeDauer = fadeEnd - fadeStart;

            int[] _startRGBA = ColorToRGBA(startColor);
            int[] _endRGBA = ColorToRGBA(endColor);

            int[] _deltaRGBA = _endRGBA.Select((elem, index) => elem - _startRGBA[index]).ToArray();
            int[] _stepRGBA = _deltaRGBA.Select(elem => (int)(elem / _fadeDauer)).ToArray();

            long _step = fadeSearched - fadeStart;
            int[] resRGBA = _startRGBA.Select((elem, index) => (int)(elem + _stepRGBA[index] * _step)).ToArray();
            Color res = Color.FromArgb((byte)resRGBA[3], (byte)resRGBA[0], (byte)resRGBA[1], (byte)resRGBA[2]);

            return res;
        }

        private int[] ColorToRGBA (Color color)
        {
            int[] res = new int[4];

            res[0] = color.R;
            res[1] = color.G;
            res[2] = color.B;
            res[3] = color.A;

            return res;
        }

        public override List<LedChangeData> LedChangeDatas(long frame)
        {
            //List<LedChangeData> res = new List<LedChangeData>();

            //long currFrame = frame - StartFrame;

            //if(!SetColorAtBeginning && currFrame >= _FadeFrames)
            //{                
            //    if(SetColorAtBeginning)
            //    {
            //        foreach(var LedID in Leds)
            //        {
            //            res.Add(new Model.LedChangeData(_Fade(StartFrame, EndFrame, frame, Colors[0], Colors[1]));
            //        }
            //    }
                
                
            //    //Get Led Colors at our (Effect) beginning
            //    _LedStatuses = App.Instance.EffectService.GetState(StartFrame, Leds, this);

            //    GotLedState = true;


            //}


            //if (currFrame >= 0 && currFrame <= EndFrame)
            //{

            //    int tmp = (int)currFrame / BlinkFrames;
            //    res.Add(new LedChangeData(Leds, Colors[tmp % Colors.Count], 0));

            //    return res;
            //}

            return null;
        }
    }
}
