using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Led.ViewModels
{
    public class ColorPickerVM : INPC
    {
        private static System.Drawing.Size _RectangleSize;        
        private static Color[,] _RectangleColors;
        public static WriteableBitmap ColorRectangle { get; set; }

        private bool _LeftMouseDown;

        public double ActualWidth { get; set; }
        public double ActualHeight { get; set; }

        public Color SelectedColor { get; set; }
        
        public Command<MouseEventArgs> MouseDownCommand { get; set; }
        public Command<MouseEventArgs> MouseMoveCommand { get; set; }
        public Command<MouseEventArgs> MouseUpCommand { get; set; }

        public Command<SizeChangedEventArgs> ImageSizeChanged { get; set; }

        static ColorPickerVM()
        {
            _RectangleSize = new System.Drawing.Size(765, 510);

            _RectangleColors = new Color[_RectangleSize.Width, _RectangleSize.Height];
            ColorRectangle = new WriteableBitmap(_RectangleSize.Width, _RectangleSize.Height, 96, 96, PixelFormats.Bgra32, null);
            System.Windows.Int32Rect _pixelRect = new System.Windows.Int32Rect(0, 0, _RectangleSize.Width, _RectangleSize.Height);
            byte[] _pixelArray = new byte[_RectangleSize.Width * _RectangleSize.Height * 4];
            int _halfHeight = _RectangleSize.Height / 2;
            int _thirdWidth = _RectangleSize.Width / 3;
            byte _red, _green, _blue;            
            double _scaleHeight, _scaleWidth;
            _red = 0;
            _green = 0;
            _blue = 0;

            for (int i = 0; i < _RectangleSize.Height; i++)
            {
                if (i / _halfHeight == 0)
                    _scaleHeight = 1 - ((double)i / _halfHeight);
                else
                    _scaleHeight = ((double)(i - _halfHeight) / _halfHeight);

                for (int j = 0; j < _RectangleSize.Width; j++)
                {
                    double _region = j / _thirdWidth;
                    if (_region >= 2)
                    {
                        _scaleWidth = 1 - ((double)(j - _thirdWidth * 2) / _thirdWidth);
                        _red = (byte)(255 * (1 - _scaleWidth));
                        _green = (byte)(255 * _scaleWidth);
                        _blue = 0;
                    }
                    else if (_region >= 1)
                    {
                        _scaleWidth = 1 - ((double)(j - _thirdWidth) / _thirdWidth);
                        _red = 0;
                        _green = (byte)(255 * (1 - _scaleWidth));
                        _blue = (byte)(255 * _scaleWidth);
                    }
                    else if (_region >= 0)
                    {
                        _scaleWidth = 1 - ((double)j / _thirdWidth);
                        _red = (byte)(255 * _scaleWidth);
                        _green = 0;
                        _blue = (byte)(255 * (1 - _scaleWidth));
                    }

                    int offset = (i * (_RectangleSize.Width * 4) + j * 4);
                    if (i / _halfHeight == 0)
                    {
                        _red = (byte)(_red + ((255 - _red) * _scaleHeight));
                        _green = (byte)(_green + ((255 - _green) * _scaleHeight));
                        _blue = (byte)(_blue + ((255 - _blue) * _scaleHeight));
                    }
                    else
                    {
                        _red = (byte)(_red - (_red * _scaleHeight));
                        _green = (byte)(_green - (_green * _scaleHeight));
                        _blue = (byte)(_blue - (_blue * _scaleHeight));
                    }                    
                                        
                    _pixelArray[offset] = _blue;
                    _pixelArray[offset + 1] = _green;
                    _pixelArray[offset + 2] = _red;
                    _pixelArray[offset + 3] = 255;
                    _RectangleColors[j, i] = Color.FromArgb(255, _red, _green, _blue);
                }
            }
            int stride = _RectangleSize.Width * 4;
            ColorRectangle.WritePixels(_pixelRect, _pixelArray, stride, 0);
        }

        public ColorPickerVM()
        {
            MouseMoveCommand = new Command<MouseEventArgs>(OnMouseMoveCommand);
            ImageSizeChanged = new Command<SizeChangedEventArgs>(OnImageSizeChanged);
        }

        public void OnMouseDownCommand(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _LeftMouseDown = true;
        }

        public void OnMouseMoveCommand(MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition((IInputElement)e.Source);
            Point mousePositionScaled = new Point(mousePosition.X * (_RectangleSize.Width / ActualWidth), mousePosition.Y * (_RectangleSize.Height / ActualHeight));
            byte[] color = new byte[4];
            color[0] = 255;
            color[1] = _RectangleColors[(int)mousePositionScaled.X, (int)mousePositionScaled.Y].B;
            color[2] = _RectangleColors[(int)mousePositionScaled.X, (int)mousePositionScaled.Y].G;
            color[3] = _RectangleColors[(int)mousePositionScaled.X, (int)mousePositionScaled.Y].R;
            Console.WriteLine("Mouse: X: {0}, Y: {1}", (int)mousePositionScaled.X, (int)mousePositionScaled.Y);
            Console.WriteLine("Color: B: {0}, G: {1}, R: {2}", color[1], color[2], color[3]);
            App.Instance.ConnectivityService.SendMessage(new Services.lib.TCP.EntityMessage(TcpMessages.Color, color), "B827EB04E40A");
        }

        public void OnMouseUpCommand(MouseEventArgs e)
        {
            _LeftMouseDown = false;
        }

        public void OnImageSizeChanged(SizeChangedEventArgs e)
        {
            Console.WriteLine("Imagesize: X: {0], Y: {1}", e.NewSize.Width, e.NewSize.Height);
        }
    }
}
