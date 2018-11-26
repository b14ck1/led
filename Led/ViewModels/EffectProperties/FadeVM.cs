using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Led.ViewModels.EffectProperties
{
    public class FadeVM : BaseVM
    {
        private Model.Effect.EffectFadeColor _EffectFadeColor { get; }

        public bool Repeat
        {
            get => _EffectFadeColor.Repeat;
            set
            {
                if (_EffectFadeColor.Repeat != value)
                {
                    _EffectFadeColor.Repeat = value;
                    RaisePropertyChanged(nameof(Repeat));

                    if (!Repeat)
                    {
                        _EffectFadeColor.FramesForOneRepetition = _EffectFadeColor.Dauer;
                        RaisePropertyChanged(nameof(FramesForOneRepetition));
                        RaisePropertyChanged(nameof(NumberOfRepetitions));
                    }
                }
            }
        }

        public int FramesForOneRepetition
        {
            get => _EffectFadeColor.FramesForOneRepetition;
            set
            {
                if (_EffectFadeColor.FramesForOneRepetition != value)
                {
                    _EffectFadeColor.FramesForOneRepetition = value;
                    RaisePropertyChanged(nameof(FramesForOneRepetition));
                    RaisePropertyChanged(nameof(NumberOfRepetitions));
                }
            }
        }

        public double NumberOfRepetitions
        {
            get => _EffectFadeColor.Dauer / _EffectFadeColor.FramesForOneRepetition;
            set
            {
                if (_EffectFadeColor.Dauer / _EffectFadeColor.FramesForOneRepetition != value)
                {
                    _EffectFadeColor.FramesForOneRepetition = (int)(_EffectFadeColor.Dauer / value);
                    RaisePropertyChanged(nameof(FramesForOneRepetition));
                    RaisePropertyChanged(nameof(NumberOfRepetitions));
                }
            }
        } 

        public FadeVM(Model.Effect.EffectFadeColor effectFadeColor)
            :base(effectFadeColor)
        {
            _EffectFadeColor = effectFadeColor;
        }
    }
}
