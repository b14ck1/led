using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Led.ViewModels.EffectProperties
{
    public class BlinkVM : BaseVM
    {
        private Model.Effect.EffectBlinkColor _EffectBlinkColor { get; }

        public ushort BlinkFrames
        {
            get => _EffectBlinkColor.BlinkFrames;
            set
            {
                if (_EffectBlinkColor.BlinkFrames != value)
                {
                    _EffectBlinkColor.BlinkFrames = value;
                    RaisePropertyChanged(nameof(BlinkFrames));
                }
            }
        }

        public BlinkVM(Model.Effect.EffectBlinkColor effectBlinkColor)
            :base(effectBlinkColor)
        {
            _EffectBlinkColor = effectBlinkColor;
        }
    }
}
