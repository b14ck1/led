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
        private static Color[,] _RectangleColors;
        public static System.Drawing.Size ImageSize { get; set; }
        public static WriteableBitmap ColorRectangle { get; set; }

        public ColorARGB CurrColor { get; set; }
        private bool _LeftMouseDown;

        public double ActualWidth { get; set; }
        public double ActualHeight { get; set; }
        
        public Command<MouseEventArgs> MouseDownCommand { get; set; }
        public Command<MouseEventArgs> MouseMoveCommand { get; set; }
        public Command<MouseEventArgs> MouseUpCommand { get; set; }

        public Command<SizeChangedEventArgs> ImageSizeChanged { get; set; }

        static ColorPickerVM()
        {
            ImageSize = new System.Drawing.Size(1530, 510);

            _RectangleColors = new Color[ImageSize.Width, ImageSize.Height];
            ColorRectangle = new WriteableBitmap(ImageSize.Width, ImageSize.Height, 96, 96, PixelFormats.Bgra32, null);
            System.Windows.Int32Rect _pixelRect = new System.Windows.Int32Rect(0, 0, ImageSize.Width, ImageSize.Height);
            byte[] _pixelArray = new byte[ImageSize.Width * ImageSize.Height * 4];
            int _halfHeight = ImageSize.Height / 2;
            int _sixtWidth = ImageSize.Width / 6;
            byte _red, _green, _blue;            
            double _scaleHeight, _scaleWidth;
            _red = 0;
            _green = 0;
            _blue = 0;

            for (int i = 0; i < ImageSize.Height; i++)
            {
                if (i / _halfHeight == 0)
                    _scaleHeight = 1 - ((double)i / _halfHeight);
                else
                    _scaleHeight = ((double)(i - _halfHeight) / _halfHeight);

                for (int j = 0; j < ImageSize.Width; j++)
                {
                    double _region = j / _sixtWidth;
                    switch (_region)
                    {
                        case 5:
                            _scaleWidth = 1 - ((double)(j - _sixtWidth * 5) / _sixtWidth);
                            _red = 255;
                            _green = (byte)(255 * _scaleWidth);
                            _blue = 0;
                            break;
                        case 4:
                            _scaleWidth = (double)(j - _sixtWidth * 4) / _sixtWidth;
                            _red = (byte)(255 * _scaleWidth);
                            _green = 255;
                            _blue = 0;
                            break;
                        case 3:
                            _scaleWidth = 1 - ((double)(j - _sixtWidth * 3) / _sixtWidth);
                            _red = 0;
                            _green = 255;
                            _blue = (byte)(255 * _scaleWidth);
                            break;
                        case 2:
                            _scaleWidth = (double)(j - _sixtWidth * 2) / _sixtWidth;
                            _red = 0;
                            _green = (byte)(255 * _scaleWidth);
                            _blue = 255;
                            break;
                        case 1:
                            _scaleWidth = 1 - ((double)(j - _sixtWidth) / _sixtWidth);
                            _red = (byte)(255 * _scaleWidth);
                            _green = 0;
                            _blue = 255;
                            break;
                        case 0:
                            _scaleWidth = (double)(j) / _sixtWidth;
                            _red = 255;
                            _green = 0;
                            _blue = (byte)(255 * _scaleWidth);
                            break;
                        default:
                            break;
                    }

                    int offset = (i * (ImageSize.Width * 4) + j * 4);
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
            int stride = ImageSize.Width * 4;
            ColorRectangle.WritePixels(_pixelRect, _pixelArray, stride, 0);
        }

        public ColorPickerVM()
        {
            MouseMoveCommand = new Command<MouseEventArgs>(OnMouseMoveCommand);
            ImageSizeChanged = new Command<SizeChangedEventArgs>(OnImageSizeChanged);

            CurrColor = new ColorARGB();
        }

        public void OnMouseDownCommand(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _LeftMouseDown = true;
        }

        public void OnMouseMoveCommand(MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition((IInputElement)e.Source);
            int _scaledX = (int)(mousePosition.X * (ImageSize.Width / ActualWidth) - 1);
            int _scaledY = (int)(mousePosition.Y * (ImageSize.Height / ActualHeight) - 1);
            if (_scaledX < 0)
                _scaledX = 0;
            else if (_scaledX > ImageSize.Width - 1)
                _scaledX = ImageSize.Width - 1;
            if (_scaledY < 0)
                _scaledY = 0;
            else if (_scaledY > ImageSize.Height - 1)
                _scaledY = ImageSize.Height - 1;

            byte[] color = new byte[4];
            color[0] = 255;
            color[1] = _RectangleColors[_scaledX, _scaledY].B;
            color[2] = _RectangleColors[_scaledX, _scaledY].G;
            color[3] = _RectangleColors[_scaledX, _scaledY].R;
            //Console.WriteLine("Mouse: X: {0}, Y: {1}", (int)mousePositionScaled.X, (int)mousePositionScaled.Y);
            //Console.WriteLine("Color: B: {0}, G: {1}, R: {2}", color[1], color[2], color[3]);
            CurrColor.SetNewColor(_RectangleColors[_scaledX, _scaledY]);
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

        public class ColorARGB : INPC
        {
            private byte _A;
            public byte A
            {
                get => _A;
                set
                {
                    if (_A != value)
                    {
                        _A = value;
                        RaisePropertyChanged(nameof(A));
                        RaisePropertyChanged(nameof(SelectedColor));
                    }
                }
            }
            private byte _R;
            public byte R
            {
                get => _R;
                set
                {
                    if (_R != value)
                    {
                        _R = value;
                        RaisePropertyChanged(nameof(R));
                        RaisePropertyChanged(nameof(SelectedColor));
                    }
                }
            }
            private byte _G;
            public byte G
            {
                get => _G;
                set
                {
                    if (_G != value)
                    {
                        _G = value;
                        RaisePropertyChanged(nameof(G));
                        RaisePropertyChanged(nameof(SelectedColor));
                    }
                }
            }
            private byte _B;
            public byte B
            {
                get => _B;
                set
                {
                    if (_B != value)
                    {
                        _B = value;
                        RaisePropertyChanged(nameof(B));
                        RaisePropertyChanged(nameof(SelectedColor));
                    }
                }
            }

            public Brush SelectedColor
            {
                get => new SolidColorBrush(Color.FromArgb(A, R, G, B));
            }

            public void SetNewColor(Color color)
            {
                _R = color.R;
                _G = color.G;
                _B = color.B;

                RaisePropertyChanged(nameof(R));
                RaisePropertyChanged(nameof(G));
                RaisePropertyChanged(nameof(B));
                RaisePropertyChanged(nameof(SelectedColor));
            }

            public ColorARGB()
            {
                A = 255;
            }
        }
    }
}
