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
        private bool _AllowSelectLeds;
        private bool _SelectingLeds;
        private LedEntityView _SelectGroupView;
        private Utility.Rectangle _Selection;

        public override List<Rectangle> FrontLedGroups
        {
            get
            {
                List<Rectangle> res = new List<Rectangle>(base.FrontLedGroups);
                if (_SelectGroupView == LedEntityView.Front)
                    res.Add(_Selection);
                return res;
            }
        }
        public override List<Rectangle> BackLedGroups
        {
            get
            {
                List<Rectangle> res = new List<Rectangle>(base.BackLedGroups);
                if (_SelectGroupView == LedEntityView.Back)
                    res.Add(_Selection);
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

        public LedEntitySelectVM(Model.LedEntity ledEntity)
            : base(ledEntity)
        {
            _selectedLeds = new List<int>();

            FrontImageMouseDownCommand = new Command<MouseEventArgs>(_OnFrontImageMouseDownCommand);
            FrontImageMouseMoveCommand = new Command<MouseEventArgs>(_OnFrontImageMouseMoveCommand);

            BackImageMouseDownCommand = new Command<MouseEventArgs>(_OnBackImageMouseDownCommand);
            BackImageMouseMoveCommand = new Command<MouseEventArgs>(_OnBackImageMouseMoveCommand);

            ImageMouseUpCommand = new Command<MouseEventArgs>(_OnImageMouseUpCommand);

            _UpdateAllLedPositions(true);            
        }

        private void _OnFrontImageMouseDownCommand(MouseEventArgs e)
        {
            if (_AllowSelectLeds)
            {
                _CreateSelectGroup(e, LedEntityView.Front);
                RaisePropertyChanged(nameof(FrontLedGroups));
            }
        }
        private void _OnBackImageMouseDownCommand(MouseEventArgs e)
        {
            if (_AllowSelectLeds)
            {
                _CreateSelectGroup(e, LedEntityView.Back);
                RaisePropertyChanged(nameof(BackLedGroups));
            }
        }

        private void _OnFrontImageMouseMoveCommand(MouseEventArgs e)
        {
            if (_SelectingLeds)
                _ResizeGroup(e);
        }
        private void _OnBackImageMouseMoveCommand(MouseEventArgs e)
        {
            if (_SelectingLeds)
                _ResizeGroup(e);
        }

        private void _OnImageMouseUpCommand(MouseEventArgs e)
        {
            _SelectingLeds = false;
            if (_Selection != null)
            {
                foreach(var ID in _ScanForLeds())
                {
                    if (!_selectedLeds.Contains(ID))
                        _selectedLeds.Add(ID);
                }
            }

            _Selection = null;
            RaisePropertyChanged(nameof(FrontLedGroups));
            RaisePropertyChanged(nameof(BackLedGroups));
        }

        private void _CreateSelectGroup(MouseEventArgs e, LedEntityView view)
        {                        
            Point mousePosition = e.GetPosition((IInputElement)e.Source);
            mousePosition = _ScalePoint(mousePosition);

            _Selection = new Utility.Rectangle((int)mousePosition.X, (int)mousePosition.Y, Defines.LedSelectRectangleColor);

            _SelectGroupView = view;
            _SelectingLeds = true;
        }

        private void _ResizeGroup(MouseEventArgs e)
        {
            if (!_SelectingLeds)
                return;

            Point mousePosition = e.GetPosition((IInputElement)e.Source);            
            mousePosition = _ScalePoint(mousePosition);

            double deltaX = mousePosition.X - _Selection.X;
            double deltaY = mousePosition.Y - _Selection.Y;

            if (deltaX < 5)
                deltaX = 5;
            if (deltaY < 5)
                deltaY = 5;

            _Selection.Width = (int)deltaX;
            _Selection.Height = (int)deltaY;

            _ScanForLeds();
        }
        
        private List<int> _ScanForLeds()
        {
            List<int> ledIndices = new List<int>();

            Point start = new Point(_Selection.X, _Selection.Y);
            Point end = new Point(start.X + _Selection.Width, start.Y + _Selection.Height);
            for (int i = 0; i < Leds.Count; i++)
            {
                LedVM led = Leds[i];
                if (led.View == _SelectGroupView && led.Position.X <= end.X && led.Position.X >= start.X && led.Position.Y <= end.Y && led.Position.Y >= start.Y)
                {
                    ledIndices.Add(i);
                    led.Brush = Defines.LedSelectedColor;
                }
                else if (!_selectedLeds.Contains(i))
                    led.Brush = Defines.LedColor;
            }

            return ledIndices;
        }

        private Utility.LedModelID _IndexToLedID(int index)
        {
            foreach (var KVP in _LedOffsets)
            {
                if (index < KVP.Value.Offset + KVP.Value.Length)
                    return new Utility.LedModelID(KVP.Key.BusID, KVP.Key.PositionInBus, (ushort)(index - KVP.Value.Offset));
            }
            return null;
        }

        private int _LedIDToIndex(LedModelID ID)
        {
            return ID.Led + _LedOffsets[_LedIDToGroupVM[new LedGroupIdentifier(ID.BusID, ID.PositionInBus)]].Offset;
        }

        private void AddEffect(ushort startFrame = 0)
        {
            LedEntity.Effects.Add(new Model.Effect.EffectSetColor(startFrame, startFrame));
            Effects.Add(new EffectBaseVM(LedEntity.Effects.Last()));

            //Entweder CurrentEffect ändern oder Message rausschicken
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
                            _SetLedColor(_selectedLeds, Colors.Blue);
                        }
                        _AllowSelectLeds = true;
                    }
                    else
                    {
                        _AllowSelectLeds = false;
                        _SelectingLeds = false;
                        _SendMessage(MediatorMessages.EffectVMEditSelectedLedsFinished, new MediatorMessageData.EffectVMEditSelectedLeds(false, SelectedLeds));
                        _SetLedColor(_selectedLeds, System.Windows.Media.Colors.Red);
                    }
                    break;
                case MediatorMessages.EffectVMEditSelectedLedsFinished:
                    _SetLedColor(_selectedLeds, Colors.LimeGreen);
                    _selectedLeds.Clear();
                    break;
                default:
                    break;
            }
        }
    }
}
