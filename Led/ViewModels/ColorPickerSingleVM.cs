using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Led.ViewModels
{
    class ColorPickerSingleVM : INPC
    {
        private Color _Color;
        public Color Color {
            get => _Color;
            set {
                if (_Color != value)
                {
                    _Color = value;
                    RaisePropertyChanged(nameof(Color));
                }
            }
        }

        public Brush Brush
        {
            get => new SolidColorBrush(Color);
        }

        public Command<MouseEventArgs> MouseDownCommand { get; set; }

        public EventHandler GetColorIssued;
        public EventHandler SetColorIssued;

        public void SetColor(Color color)
        {
            Color = color;
        }

        public ColorPickerSingleVM()
        {
            Color = Colors.Transparent;            
        }

        public ColorPickerSingleVM(Color color)
        {
            Color = color;
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
