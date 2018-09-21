using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Led.ViewModels.EffectProperties
{
    class FadeVM : INPC
    {
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

        public FadeVM(List<Color> colors)
        {
            Colors = new ObservableCollection<EffectColorVM>();
            for (int i = 0; i < colors.Count; i++)
            {
                Colors.Add(new EffectColorVM(colors[i], "Farbe " + (i + 1)));
            }
        }
    }
}
