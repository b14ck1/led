using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Led.ViewModels
{
    public class LedVM : INPC
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

        public LedEntityView View { get; set; }

        public LedVM(Point position, LedEntityView view)
        {
            Position = position;
            Brush = Defines.LedColor;
            View = view;
        }
    }
}
