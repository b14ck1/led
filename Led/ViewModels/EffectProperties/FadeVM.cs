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

        public FadeVM()
        {
            Colors = new ObservableCollection<EffectColorVM>()
            {
                new EffectColorVM(System.Windows.Media.Colors.Black, "Farbe 1"),
                new EffectColorVM(System.Windows.Media.Colors.Black, "Farbe 2")
            };
        }
    }
}
