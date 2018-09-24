using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Led.ViewModels.EffectProperties
{
    public class EffectColorVM : INPC
    {
        private System.Windows.Media.Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    RaisePropertyChanged(nameof(ColorAsBrush));
                }
            }
        }
        public Brush ColorAsBrush => new SolidColorBrush(_color);

        private string _number;
        public string Number
        {
            get => _number;
            set
            {
                if (_number != value)
                {
                    _number = value;
                    RaisePropertyChanged(nameof(Number));
                }
            }
        }

        public Command PickColorCommand { get; set; }

        public EffectColorVM(Color color, string number)
        {
            Color = color;
            Number = number;

            PickColorCommand = new Command(_OnPickColorCommand);
        }

        private void _OnPickColorCommand()
        {
            ColorPickerWPF.ColorPickerWindow.ShowDialog(out Color color);
            Color = color;
        }
    }
}
