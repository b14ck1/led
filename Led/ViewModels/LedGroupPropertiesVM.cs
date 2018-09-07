using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Led.ViewModels
{
    class LedGroupPropertiesVM : INPC
    {
        private Model.LedGroup _ledGroup;
        public Model.LedGroup LedGroup
        {
            get => _ledGroup;
            set
            {
                if (_ledGroup != value)
                {
                    _ledGroup = value;
                    RaiseAllPropertyChanged();
                }
            }
        }

        public LedEntityView View
        {
            get => _ledGroup.View.View;
            set
            {
                if (_ledGroup.View.View != value)
                {
                    _ledGroup.View.View = value;
                    RaisePropertyChanged(nameof(View));
                }
            }
        }

        public byte BusID
        {
            get => _ledGroup.BusID;
            set
            {
                if (value != _ledGroup.BusID)
                {
                    _ledGroup.BusID = value;
                    RaisePropertyChanged(nameof(BusID));
                }
            }
        }
        public byte PositionInBus
        {
            get => _ledGroup.PositionInBus;
            set
            {
                if (value != _ledGroup.PositionInBus)
                {
                    _ledGroup.PositionInBus = value;
                    RaisePropertyChanged(nameof(PositionInBus));
                }
            }
        }

        public int PositionInEntityX
        {
            get => (int)_ledGroup.PositionInEntity.X;
            set
            {
                _ledGroup.PositionInEntity = new System.Windows.Point(value, _ledGroup.PositionInEntity.Y);
                RaisePropertyChanged(nameof(PositionInEntityX));
            }
        }
        public int PositionInEntityY
        {
            get => (int)_ledGroup.PositionInEntity.Y;
            set
            {
                _ledGroup.PositionInEntity = new System.Windows.Point(_ledGroup.PositionInEntity.X, value);
                RaisePropertyChanged(nameof(PositionInEntityY));
            }
        }

        public List<LedGridCellVM> LedGrid
        {
            get
            {
                List<LedGridCellVM> res = new List<LedGridCellVM>();
                for (int j = 0; j < GridRangeY; j++)
                {
                    for (int i = 0; i < GridRangeX; i++)
                    {
                        res.Add(new LedGridCellVM(_ledGroup.View.LedGrid[i, j]));
                        res.Last().PropertyChanged += LedGroupViewModel_PropertyChanged;
                    }
                }
                return res;
            }
        }
        public PointCollection WiringLine
        {
            get
            {
                PointCollection res = new PointCollection();
                _ledGroup.Leds = new List<Point>();

                int i = StartPositionWiringX;
                int j = StartPositionWiringY;
                int iterations = 0;
                LedViewArrowDirection Direction = LedViewArrowDirection.None;

                while (i >= 0 && j >= 0 && i < GridRangeX && j < GridRangeY && iterations < 1000)
                {
                    if (_ledGroup.View.LedGrid[i, j].Direction != LedViewArrowDirection.None)
                        Direction = _ledGroup.View.LedGrid[i, j].Direction;

                    res.Add(new Point(i * 36 + 15 + 3, j * 36 + 15 + 11));

                    switch (Direction)
                    {
                        case LedViewArrowDirection.Up:
                            j--;
                            break;
                        case LedViewArrowDirection.Right:
                            i++;
                            break;
                        case LedViewArrowDirection.Down:
                            j++;
                            break;
                        case LedViewArrowDirection.Left:
                            i--;
                            break;
                        case LedViewArrowDirection.None:
                            return null;
                    }

                    iterations++;
                }
                return res;
            }
        }

        public int GridRangeX
        {
            get => _ledGroup.View.LedGrid.GetLength(0);
            set
            {
                if (_ledGroup.View.LedGrid.GetLength(0) != value)
                {
                    _ledGroup.View.LedGrid = new Model.LedGridCell[value, _ledGroup.View.LedGrid.GetLength(1)];
                    for (int i = 0; i < GridRangeX; i++)
                    {
                        for (int j = 0; j < GridRangeY; j++)
                        {
                            _ledGroup.View.LedGrid[i, j] = new Model.LedGridCell();
                        }
                    }
                    RaisePropertyChanged(nameof(GridRangeX));
                    RaisePropertyChanged(nameof(LedGrid));
                }
            }
        }
        public int GridRangeY
        {
            get => _ledGroup.View.LedGrid.GetLength(1);
            set
            {
                if (_ledGroup.View.LedGrid.GetLength(1) != value)
                {
                    _ledGroup.View.LedGrid = new Model.LedGridCell[_ledGroup.View.LedGrid.GetLength(0), value];
                    for (int i = 0; i < GridRangeX; i++)
                    {
                        for (int j = 0; j < GridRangeY; j++)
                        {
                            _ledGroup.View.LedGrid[i, j] = new Model.LedGridCell();
                        }
                    }
                    RaisePropertyChanged(nameof(GridRangeY));
                    RaisePropertyChanged(nameof(LedGrid));
                }
            }
        }

        public int StartPositionWiringX
        {
            get => (int)_ledGroup.View.StartPositionWiring.X;
            set
            {
                if (_ledGroup.View.StartPositionWiring.X != value)
                {
                    _ledGroup.View.StartPositionWiring = new Point(value, _ledGroup.View.StartPositionWiring.Y);
                    RaisePropertyChanged(nameof(StartPositionWiringX));
                    RaisePropertyChanged(nameof(WiringLine));
                }
            }
        }
        public int StartPositionWiringY
        {
            get => (int)_ledGroup.View.StartPositionWiring.Y;
            set
            {
                if (_ledGroup.View.StartPositionWiring.Y != value)
                {
                    _ledGroup.View.StartPositionWiring = new Point(_ledGroup.View.StartPositionWiring.X, value);
                    RaisePropertyChanged(nameof(StartPositionWiringY));
                    RaisePropertyChanged(nameof(WiringLine));
                }
            }
        }

        public Point StartPositionOnImage
        {
            get => _ledGroup.View.StartPositionOnImage;
            set
            {
                if (_ledGroup.View.StartPositionOnImage != value)
                {
                    _ledGroup.View.StartPositionOnImage = value;
                    RaisePropertyChanged(nameof(StartPositionOnImage));
                }
            }
        }
        public Point StartPositionOnImageScaled {
            set
            {
                if (_Rectangle.X != value.X || _Rectangle.Y != value.Y)
                {
                    _Rectangle.X = (int)value.X;
                    _Rectangle.Y = (int)value.Y;
                }
            }
        }

        public Size SizeOnImage
        {
            get => _ledGroup.View.SizeOnImage;
            set
            {
                if (_ledGroup.View.SizeOnImage != value)
                {
                    _ledGroup.View.SizeOnImage = value;
                    RaisePropertyChanged(nameof(SizeOnImage));
                }
            }
        }
        public Size SizeOnImageScaled
        {            
            set
            {
                if (_Rectangle.Width != value.Width || _Rectangle.Height != value.Height)
                {
                    _Rectangle.Width = (int)value.Width;
                    _Rectangle.Height = (int)value.Height;                    
                }
            }
        }

        private Utility.Rectangle _Rectangle;
        public Utility.Rectangle Rectangle
        {
            get => _Rectangle;
            set
            {
                if (_Rectangle != value)
                {
                    _Rectangle = value;
                    RaisePropertyChanged(nameof(Rectangle));
                }
            }
        }

        public Brush Stroke { get => Defines.LedGroupColor; }

        private Point _minLedPosition
        {
            get
            {
                int _minX = 0;
                int _minY = 0;

                for (int i = 0; i < GridRangeX; i++)
                {
                    bool found = false;
                    for (int j = 0; j < GridRangeY; j++)
                    {
                        if (_ledGroup.View.LedGrid[i, j].Status)
                        {
                            _minX = i;
                            found = true;
                            break;
                        }
                    }

                    if (found)
                        break;
                }

                for (int j = 0; j < GridRangeY; j++)
                {
                    bool found = false;
                    for (int i = 0; i < GridRangeX; i++)
                    {
                        if (_ledGroup.View.LedGrid[i, j].Status)
                        {
                            _minY = j;
                            found = true;
                            break;
                        }
                    }

                    if (found)
                        break;
                }

                return new Point(_minX, _minY);
            }
        }

        public Command CloseWindowCommand { get; set; }

        public LedGroupPropertiesVM(Model.LedGroup ledGroup = null)
        {
            LedGroup = ledGroup ?? (LedGroup = new Model.LedGroup());
            Rectangle = new Utility.Rectangle(0, 0, Defines.LedGroupColor);
            CloseWindowCommand = new Command(OnCloseWindowCommand);
        }

        private void OnCloseWindowCommand()
        {
            _ledGroup.Leds = new List<Point>();
            Point _minLeds = _minLedPosition;
            bool[,] done = new bool[GridRangeX, GridRangeY];

            int i = StartPositionWiringX;
            int j = StartPositionWiringY;
            int iterations = 0;
            LedViewArrowDirection Direction = LedViewArrowDirection.None;


            while (i >= 0 && j >= 0 && i < GridRangeX && j < GridRangeY && iterations < 10000)
            {
                if (_ledGroup.View.LedGrid[i, j].Direction != LedViewArrowDirection.None)
                    Direction = _ledGroup.View.LedGrid[i, j].Direction;

                if (_ledGroup.View.LedGrid[i, j].Status && !done[i, j] && Direction != LedViewArrowDirection.None)
                {
                    _ledGroup.Leds.Add(new Point(i - _minLeds.X, j - _minLeds.Y));
                    done[i, j] = true;
                }

                switch (Direction)
                {
                    case LedViewArrowDirection.Up:
                        j--;
                        break;
                    case LedViewArrowDirection.Right:
                        i++;
                        break;
                    case LedViewArrowDirection.Down:
                        j++;
                        break;
                    case LedViewArrowDirection.Left:
                        i--;
                        break;
                    case LedViewArrowDirection.None:
                        i = -1;
                        break;
                }

                iterations++;
            }
            _ledGroup.View.GridLedStartOffset = _minLeds;

            App.Instance.WindowService.CloseWindow(this);
        }

        private void LedGroupViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(LedGridCellVM.Arrow)))
                RaisePropertyChanged(nameof(WiringLine));
        }
    }
}
