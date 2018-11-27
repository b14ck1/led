using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Led.ViewModels
{
    public class ColorPickerSingleVM : INPC
    {
        private ColorPickerVM.ColorARGB _CurrColor;
        public ColorPickerVM.ColorARGB CurrColor {
            get => _CurrColor;
            set {
                if (_CurrColor != value)
                {
                    _CurrColor = value;
                    RaisePropertyChanged(nameof(Brush));
                }
            }
        }

        public Brush Brush
        {
            get => new SolidColorBrush(CurrColor.ColorScaled);
        }

        public Command<MouseEventArgs> MouseDownCommand { get; set; }

        public event EventHandler GetColorIssued;
        public event EventHandler SetColorIssued;

        public void SetColor(Color color)
        {
            CurrColor.SetNewColor(color, true);
            RaisePropertyChanged(nameof(Brush));
        }

        public ColorPickerSingleVM()
        {
            CurrColor = new ColorPickerVM.ColorARGB(Colors.Transparent);

            MouseDownCommand = new Command<MouseEventArgs>(_OnMouseDownCommand);
        }

        public ColorPickerSingleVM(Color color)
        {
            CurrColor = new ColorPickerVM.ColorARGB(color);

            MouseDownCommand = new Command<MouseEventArgs>(_OnMouseDownCommand);
        }

        private void _OnMouseDownCommand(MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                GetColorIssued?.Invoke(this, null);
            }
            else if(e.RightButton == MouseButtonState.Pressed)
            {
                SetColorIssued?.Invoke(this, null);
            }
        }

        public enum ColorType
        {
            global,
            entity
        }
    }
}
