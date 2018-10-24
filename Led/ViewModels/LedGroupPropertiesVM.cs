using Led.Interfaces;
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
    public class LedGroupPropertiesVM : INPC, IParticipant
    {
        private Services.MediatorService _Mediator;

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

        /// <summary>
        /// On which view is this group located.
        /// </summary>
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

        /// <summary>
        /// On which bus is this group located.
        /// </summary>
        public byte BusID
        {
            get => _ledGroup.BusID;
            set
            {
                if (value != _ledGroup.BusID)
                {
                    _ledGroup.BusID = value;
                    RaisePropertyChanged(nameof(BusID));
                    _SendMessage(MediatorMessages.LedEntityCRUDVM_GroupBusDefinitionsChanged, null);
                }
            }
        }
        /// <summary>
        /// Which position has the group on the bus.
        /// </summary>
        public byte PositionInBus
        {
            get => _ledGroup.PositionInBus;
            set
            {
                if (value != _ledGroup.PositionInBus)
                {
                    _ledGroup.PositionInBus = value;
                    RaisePropertyChanged(nameof(PositionInBus));
                    _SendMessage(MediatorMessages.LedEntityCRUDVM_GroupBusDefinitionsChanged, null);
                }

            }
        }
        private bool _NeedCorrectionBusDefinitions;        
        public Brush BusBorder => _NeedCorrectionBusDefinitions ? Defines.LedGroupColorWrong : SystemColors.ControlDarkBrush;

        private bool _NeedCorrectionPosition;
        public Brush PositionBorder => _NeedCorrectionPosition ? Defines.LedGroupColorWrong : SystemColors.ControlDarkBrush;

        /// <summary>
        /// Which x-y-position does this group have in the whole entity
        /// /// </summary>
        public int PositionInEntityX
        {
            get => (int)_ledGroup.PositionInEntity.X;
            set
            {
                _ledGroup.PositionInEntity = new System.Windows.Point(value, _ledGroup.PositionInEntity.Y);
                RaisePropertyChanged(nameof(PositionInEntityX));
                _SendMessage(MediatorMessages.LedEntityCRUDVM_GroupPositionChanged, null);
            }
        }
        /// <summary>
        /// Which x-y-position does this group have in the whole entity
        /// /// </summary>
        public int PositionInEntityY
        {
            get => (int)_ledGroup.PositionInEntity.Y;
            set
            {
                _ledGroup.PositionInEntity = new System.Windows.Point(_ledGroup.PositionInEntity.X, value);
                RaisePropertyChanged(nameof(PositionInEntityY));
                _SendMessage(MediatorMessages.LedEntityCRUDVM_GroupPositionChanged, null);
            }
        }

        /// <summary>
        /// A list with all possible Positions of the Group.
        /// Its filled X-first.
        /// </summary>
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
                        res.Last().PropertyChanged += _LedGroupViewModel_PropertyChanged;
                    }
                }
                return res;
            }
        }
        /// <summary>
        /// A list of the physical wiring of the group.
        /// </summary>
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

        /// <summary>
        /// How many positions do we got in X-direction.
        /// </summary>
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
        /// <summary>
        /// How many positions do we got in Y-direction.
        /// </summary>
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

        /// <summary>
        /// Where does the wiring of the physical group start.
        /// </summary>
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
        /// <summary>
        /// Where does the wiring of the physical group start.
        /// </summary>
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

        /// <summary>
        /// Where does the group start on the Image.
        /// </summary>
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
        /// <summary>
        /// Where does the group start on the Image on the home screen (scaled).
        /// </summary>
        public Point StartPositionOnImageScaled
        {
            set
            {
                if (_Rectangle.X != value.X || _Rectangle.Y != value.Y)
                {
                    _Rectangle.X = (int)value.X;
                    _Rectangle.Y = (int)value.Y;
                }
            }
        }

        /// <summary>
        /// How big is the group on the picture.
        /// </summary>
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
        /// <summary>
        /// How big is the group on the on the picture on the home screen (scaled).
        /// </summary>
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
        /// <summary>
        /// Dimensions of the group on the picture.
        /// </summary>
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

        private Point _MinLedPosition
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
            CloseWindowCommand = new Command(_OnCloseWindowCommand);

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);            
        }

        private void _OnCloseWindowCommand()
        {
            _ledGroup.Leds = new List<Point>();
            Point _minLeds = _MinLedPosition;
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

        private void _LedGroupViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(LedGridCellVM.Arrow)))
                RaisePropertyChanged(nameof(WiringLine));
        }

        private void _SetRectangleBorder()
        {
            if(_NeedCorrectionBusDefinitions || _NeedCorrectionPosition)
                Rectangle.Stroke = Defines.LedGroupColorWrong;
            else
                Rectangle.Stroke = Defines.LedGroupColor;
        }

        private void _SendMessage(MediatorMessages message, object data)
        {
            _Mediator.BroadcastMessage(message, this, data);
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.LedEntityCRUDVM_GroupBusDefinitionsNeedCorrectionChanged:
                    if ((data as MediatorMessageData.LedEntityCRUDVM_GroupBusDefinitionsNeedCorrectionChanged).NeedCorrection.ContainsKey(this))
                    {
                        _NeedCorrectionBusDefinitions = (data as MediatorMessageData.LedEntityCRUDVM_GroupBusDefinitionsNeedCorrectionChanged).NeedCorrection[this];
                        _SetRectangleBorder();
                        RaisePropertyChanged(nameof(BusBorder));
                    }                    
                    break;
                case MediatorMessages.LedEntityCRUDVM_GroupPositionNeedCorrectionChanged:
                    if ((data as MediatorMessageData.LedEntityCRUDVM_GroupPositionNeedCorrectionChanged).NeedCorrection.ContainsKey(this))
                    {
                        _NeedCorrectionPosition = (data as MediatorMessageData.LedEntityCRUDVM_GroupPositionNeedCorrectionChanged).NeedCorrection[this];
                        _SetRectangleBorder();
                        RaisePropertyChanged(nameof(PositionBorder));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}