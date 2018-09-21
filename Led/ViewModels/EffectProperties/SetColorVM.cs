using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels.EffectProperties
{
    class SetColorVM : INPC
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

        public SetColorVM(System.Windows.Media.Color color)
        {
            Colors = new ObservableCollection<EffectColorVM>()
            {
                new EffectColorVM(color, "Farbe")
            };
        }
    }
}
