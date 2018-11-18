using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Led.ViewModels
{
    public class ColorPickerVM : INPC
    {
        private static Size _RectangleSize;
        private static Brush[,] _RectangleBrushes;
        private static Color[,] _RectangleColors;
        public static Bitmap ColorRectangle { get; set; }

        private bool _LeftMouseDown;        

        public Color SelectedColor { get; set; }

        public Command MouseDownCommand { get; set; }
        public Command MouseMoveCommand { get; set; }
        public Command MouseUpCommand { get; set; }

        static ColorPickerVM()
        {
            _RectangleSize = new Size(765, 255);
            ColorRectangle = new Bitmap(_RectangleSize.Width, _RectangleSize.Height);
            _RectangleBrushes = new Brush[_RectangleSize.Width, _RectangleSize.Height];
            _RectangleColors = new Color[_RectangleSize.Width, _RectangleSize.Height];

            int _halfHeight = _RectangleSize.Height / 2;
            int _red, _green, _blue;

            using (Graphics g = Graphics.FromImage(ColorRectangle))
            {
                for (int i = 0; i < _RectangleSize.Width; i++)
                {
                    _red = 255;
                    _green = 0;
                    _blue = 0;
                    for (int j = 0; j < _RectangleSize.Height; j++)
                    {
                        if (j / _halfHeight == 0)
                            _RectangleColors[i, j] = Color.FromArgb(_red, _green, _blue);
                        else
                        {
                            double _scaleHeight = 1 - ((j - _RectangleSize.Height) / _RectangleSize.Height);
                            _RectangleColors[i, j] = Color.FromArgb((int)(_red * _scaleHeight), (int)(_green * _scaleHeight), (int)(_blue * _scaleHeight));
                        }
                        _RectangleBrushes[i, j] = new SolidBrush(_RectangleColors[i, j]);
                        g.FillRectangle(_RectangleBrushes[i, j], i, j, 1, 1);
                    }
                }
            }
        }

        public ColorPickerVM()
        {

        }

        public void OnMouseDownCommand(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _LeftMouseDown = true;
        }

        public void OnMouseMoveCommand(MouseEventArgs e)
        {

        }

        public void OnMouseUpCommand(MouseEventArgs e)
        {
            _LeftMouseDown = false;
        }
    }
}
