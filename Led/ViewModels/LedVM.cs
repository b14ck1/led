using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Led.ViewModels
{
    class LedVM : INPC
    {
        private Point _position;
        public Point Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    RaisePropertyChanged("Position");
                }
            }
        }

        private Brush _brush;
        public Brush Brush
        {
            get => _brush;
            set
            {
                if (_brush != value)
                {
                    _brush = value;
                    RaisePropertyChanged("Brush");
                }
            }
        }

        public LedEntityView View;

        public LedVM(Point Position, LedEntityView View)
        {
            this.Position = Position;
            Brush = Defines.LedColor;
            this.View = View;
        }
    }
}
