using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Led.ViewModels
{
    public class LedGridCellVM : INPC
    {
        public Model.LedGridCell LedView { get; }

        public bool Status
        {
            get => LedView.Status;
            set
            {
                if (LedView.Status != value)
                {
                    LedView.Status = value;
                    RaisePropertyChanged("Brush");
                }
            }
        }
        public Brush Brush
        {
            get => Status ? Defines.LedColor : Brushes.Transparent;
        }

        private LedViewArrowDirection _Direction
        {
            get => LedView.Direction;
            set
            {
                if (LedView.Direction != value)
                {
                    LedView.Direction = value;
                    RaisePropertyChanged("Arrow");
                }
            }
        }
        public PointCollection Arrow
        {
            get
            {
                switch (_Direction)
                {
                    case LedViewArrowDirection.Up: return _ArrowUp;
                    case LedViewArrowDirection.Right: return _ArrowRight;
                    case LedViewArrowDirection.Down: return _ArrowDown;
                    case LedViewArrowDirection.Left: return _ArrowLeft;
                    case LedViewArrowDirection.None: return null;
                }
                return null;
            }
        }

        public Command<MouseEventArgs> MouseDownCommand { get; set; }

        public LedGridCellVM()
        {
            LedView = new Model.LedGridCell();

            MouseDownCommand = new Command<MouseEventArgs>(_OnMouseDownCommand);
        }

        public LedGridCellVM(Model.LedGridCell _ledView)
        {
            this.LedView = _ledView;

            MouseDownCommand = new Command<MouseEventArgs>(_OnMouseDownCommand);
        }

        private void _OnMouseDownCommand(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Status = !Status;

            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if (Status)
                {
                    if (_Direction == LedViewArrowDirection.None)
                        _Direction = 0;
                    else
                        _Direction++;
                }
            }
        }

        private PointCollection _ArrowUp
        {
            get
            {
                return new PointCollection
                {
                    new System.Windows.Point(18, 30),
                    new System.Windows.Point(18, 6),
                    new System.Windows.Point(23, 11),                    
                    new System.Windows.Point(13, 11),
                    new System.Windows.Point(18, 6)
                };
            }
        }

        private PointCollection _ArrowRight
        {
            get
            {
                return new PointCollection
                {
                    new System.Windows.Point(6, 18),
                    new System.Windows.Point(30, 18),
                    new System.Windows.Point(25, 13),                    
                    new System.Windows.Point(25, 23),
                    new System.Windows.Point(30, 18)
                };
            }
        }

        private PointCollection _ArrowDown
        {
            get
            {
                return new PointCollection
                {
                    new System.Windows.Point(18, 6),
                    new System.Windows.Point(18, 30),
                    new System.Windows.Point(13, 25),
                    new System.Windows.Point(22, 25),
                    new System.Windows.Point(18, 30)                    
                };
            }
        }

        private PointCollection _ArrowLeft
        {
            get
            {
                return new PointCollection
                {
                    new System.Windows.Point(30, 18),
                    new System.Windows.Point(6, 18),
                    new System.Windows.Point(11, 13),
                    new System.Windows.Point(11, 23),
                    new System.Windows.Point(5, 18)
                };
            }
        }
    }
}
