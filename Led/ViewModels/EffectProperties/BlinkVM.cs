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
        private ushort _blinkFrames;
        public ushort BlinkFrames
        {
            get => _blinkFrames;
            set
            {
                if (_blinkFrames  != value)
                {
                    _blinkFrames  = value;
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

        public BlinkVM(List<Color> colors)
        {
            Colors = new ObservableCollection<EffectColorVM>();
            for (int i = 0; i < colors.Count; i++)
            {
                Colors.Add(new EffectColorVM(colors[i], "Farbe " + (i + 1)));
            }
        }
    }
}
