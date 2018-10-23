using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Led.ViewModels.EffectProperties
{
    public class BlinkVM : INPC
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

        private ObservableCollection<EffectColorVM> _colors;
        public ObservableCollection<EffectColorVM> Colors
        {
            get => _colors;
            set
            {
                if (_colors != value)
                {
                    _colors = value;
                    RaisePropertyChanged(nameof(Colors));
                }
            }
        }

        public BlinkVM(Model.Effect.EffectBlinkColor effectBlinkColor)
        {
            _EffectBlinkColor = effectBlinkColor;
;

            Colors = new ObservableCollection<EffectColorVM>();
            for (int i = 0; i < effectBlinkColor.Colors.Count; i++)
            {
                Colors.Add(new EffectColorVM(effectBlinkColor as Model.Effect.EffectBase, i));
            }
        }
    }
}
