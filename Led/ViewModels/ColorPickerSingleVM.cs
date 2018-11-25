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
        private Color _Color;
        public Color Color {
            get => _Color;
            set {
                if (_Color != value)
                {
                    _Color = value;
                    RaisePropertyChanged(nameof(Brush));
                }
            }
        }

        public Brush Brush
        {
            get => new SolidColorBrush(Color.FromArgb((byte)(Math.Round(Color.A * (double)255 / 31)), Color.R, Color.G, Color.B));
        }

        public Command<MouseEventArgs> MouseDownCommand { get; set; }

        public event EventHandler GetColorIssued;
        public event EventHandler SetColorIssued;

        public void SetColor(Color color)
        {
            Color = color;
        }

        public ColorPickerSingleVM()
        {
            Color = Colors.Transparent;

            MouseDownCommand = new Command<MouseEventArgs>(_OnMouseDownCommand);
        }

        public ColorPickerSingleVM(Color color)
        {
            Color = color;

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
    }
}
