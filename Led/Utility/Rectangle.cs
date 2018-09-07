using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Led.Utility
{
    public class Rectangle : INPC
    {
        private int _xPosition;
        public int X
        {
            get => _xPosition;
            set
            {
                if (_xPosition != value)
                {
                    _xPosition = value;
                    RaisePropertyChanged(nameof(X));
                }
            }
        }

        private int _yPosition;
        public int Y
        {
            get => _yPosition;
            set
            {
                if (_yPosition != value)
                {
                    _yPosition = value;
                    RaisePropertyChanged(nameof(Y));
                }
            }
        }

        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    RaisePropertyChanged(nameof(Width));
                }
            }
        }

        private int _height;
        public int Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    RaisePropertyChanged(nameof(Height));
                }
            }
        }

        private Brush _stroke;
        public Brush Stroke
        {
            get => _stroke;
            set
            {
                if (_stroke != value)
                {
                    _stroke = value;
                    RaisePropertyChanged(nameof(Stroke));
                }
            }
        }

        public LedEntityView View { get; set; }

        public Rectangle(int x, int y, Brush stroke, int width = 0, int height = 0)
        {
            X = x;
            Y = y;
            Stroke = stroke;
            Width = width;
            Height = height;
        }
    }
}
