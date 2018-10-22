using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Led.ViewModels.EffectProperties
{
    public class FadeVM : INPC
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

        public int Dauer
        {
            get => _EffectFadeColor.Dauer;
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

        public FadeVM(Model.Effect.EffectFadeColor effectFadeColor)
        {
            _EffectFadeColor = effectFadeColor;

            Colors = new ObservableCollection<EffectColorVM>();
            for (int i = 0; i < effectFadeColor.Colors.Count; i++)
            {
                Colors.Add(new EffectColorVM((effectFadeColor as Model.Effect.EffectBase), i));
            }
        }
    }
}
