using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Led.Utility;

namespace Led.ViewModels
{
    class LedEntitySelectVM : LedEntityBaseVM
    {
        private bool _allowSelectLeds;
        private bool _selectingLeds;
        private LedEntityView _selectGroupView;

        private Utility.Rectangle _selection;
        public override List<Rectangle> FrontLedGroups
        {
            get
            {
                List<Rectangle> res = new List<Rectangle>(base.FrontLedGroups);
                if (_selectGroupView == LedEntityView.Front)
                    res.Add(_selection);
                return res;
            }
        }
        public override List<Rectangle> BackLedGroups
        {
            get
            {
                List<Rectangle> res = new List<Rectangle>(base.BackLedGroups);
                if (_selectGroupView == LedEntityView.Back)
                    res.Add(_selection);
                return res;
            }
        }

        private List<int> _selectedLeds;
        public List<Utility.LedModelID> SelectedLeds
        {
            get
            {
                List<Utility.LedModelID> res = new List<Utility.LedModelID>();
                foreach (int index in _selectedLeds)
                {
                    res.Add(_IndexToLedID(index));
                }

                return res;
            }
        }

        public LedEntitySelectVM(Model.LedEntity LedEntity)
            : base(LedEntity)
        {
            _selectedLeds = new List<int>();

            FrontImageMouseDownCommand = new Command<MouseEventArgs>(OnFrontImageMouseDownCommand);
            FrontImageMouseMoveCommand = new Command<MouseEventArgs>(OnFrontImageMouseMoveCommand);

            BackImageMouseDownCommand = new Command<MouseEventArgs>(OnBackImageMouseDownCommand);
            BackImageMouseMoveCommand = new Command<MouseEventArgs>(OnBackImageMouseMoveCommand);

            ImageMouseUpCommand = new Command<MouseEventArgs>(OnImageMouseUpCommand);

            UpdateAllLedPositions(true);            
        }

        private void OnFrontImageMouseDownCommand(MouseEventArgs e)
        {
            if (_allowSelectLeds)
            {
                _CreateSelectGroup(e, LedEntityView.Front);
                RaisePropertyChanged(nameof(FrontLedGroups));
            }
        }
        private void OnBackImageMouseDownCommand(MouseEventArgs e)
        {
            if (_allowSelectLeds)
            {
                _CreateSelectGroup(e, LedEntityView.Back);
                RaisePropertyChanged(nameof(BackLedGroups));
            }
        }

        private void OnFrontImageMouseMoveCommand(MouseEventArgs e)
        {
            if (_selectingLeds)
                _ResizeGroup(e);
        }
        private void OnBackImageMouseMoveCommand(MouseEventArgs e)
        {
            if (_selectingLeds)
                _ResizeGroup(e);
        }

        private void OnImageMouseUpCommand(MouseEventArgs e)
        {
            _selectingLeds = false;
            if (_selection != null)
            {
                foreach(var ID in _ScanForLeds())
                {
                    if (!_selectedLeds.Contains(ID))
                        _selectedLeds.Add(ID);
                }
            }

            _selection = null;
            RaisePropertyChanged(nameof(FrontLedGroups));
            RaisePropertyChanged(nameof(BackLedGroups));
        }

        private void _CreateSelectGroup(MouseEventArgs e, LedEntityView View)
        {                        
            Point MousePosition = e.GetPosition((IInputElement)e.Source);
            MousePosition = _ScalePoint(MousePosition);

            _selection = new Utility.Rectangle((int)MousePosition.X, (int)MousePosition.Y, Defines.LedSelectRectangleColor);

            _selectGroupView = View;
            _selectingLeds = true;
        }

        private void _ResizeGroup(MouseEventArgs e)
        {
            if (!_selectingLeds)
                return;

            Point MousePosition = e.GetPosition((IInputElement)e.Source);            
            MousePosition = _ScalePoint(MousePosition);

            double deltaX = MousePosition.X - _selection.X;
            double deltaY = MousePosition.Y - _selection.Y;

            if (deltaX < 5)
                deltaX = 5;
            if (deltaY < 5)
                deltaY = 5;

            _selection.Width = (int)deltaX;
            _selection.Height = (int)deltaY;

            _ScanForLeds();
        }
        
        private List<int> _ScanForLeds()
        {
            List<int> LedIndices = new List<int>();

            Point Start = new Point(_selection.X, _selection.Y);
            Point End = new Point(Start.X + _selection.Width, Start.Y + _selection.Height);
            for (int i = 0; i < Leds.Count; i++)
            {
                LedVM Led = Leds[i];
                if (Led.View == _selectGroupView && Led.Position.X <= End.X && Led.Position.X >= Start.X && Led.Position.Y <= End.Y && Led.Position.Y >= Start.Y)
                {
                    LedIndices.Add(i);
                    Led.Brush = Defines.LedSelectedColor;
                }
                else if (!_selectedLeds.Contains(i))
                    Led.Brush = Defines.LedColor;
            }

            return LedIndices;
        }

        private Utility.LedModelID _IndexToLedID(int index)
        {
            foreach (var KVP in LedOffsets)
            {
                if (index < KVP.Value.Offset + KVP.Value.Length)
                    return new Utility.LedModelID(KVP.Key.BusID, KVP.Key.PositionInBus, (ushort)(index - KVP.Value.Offset));
            }
            return null;
        }

        private int _LedIDToIndex(LedModelID ID)
        {
            return ID.Led + LedOffsets[LedIDToGroupVM[new LedGroupIdentifier(ID.BusID, ID.PositionInBus)]].Offset;
        }

        public override void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.EffectVMEditSelectedLedsClicked:
                    MediatorMessageData.EffectVMEditSelectedLeds effectVMEditSelectedLedsClicked = (data as MediatorMessageData.EffectVMEditSelectedLeds);
                    if (effectVMEditSelectedLedsClicked.Edit)
                    {
                        if (effectVMEditSelectedLedsClicked.SelectedLeds != null)
                        {
                            foreach (var ID in effectVMEditSelectedLedsClicked.SelectedLeds)
                            {
                                if (!_selectedLeds.Contains(_LedIDToIndex(ID)))
                                {
                                    _selectedLeds.Add(_LedIDToIndex(ID));                                                                        
                                }
                            }
                            SetLedColor(_selectedLeds, Colors.Blue);
                        }
                        _allowSelectLeds = true;
                    }
                    else
                    {
                        _allowSelectLeds = false;
                        _selectingLeds = false;
                        _SendMessage(MediatorMessages.EffectVMEditSelectedLedsFinished, new MediatorMessageData.EffectVMEditSelectedLeds(false, SelectedLeds));
                        SetLedColor(_selectedLeds, System.Windows.Media.Colors.Red);
                    }
                    break;
                case MediatorMessages.EffectVMEditSelectedLedsFinished:
                    SetLedColor(_selectedLeds, Colors.LimeGreen);
                    _selectedLeds.Clear();
                    break;
                default:
                    break;
            }
        }
    }
}
