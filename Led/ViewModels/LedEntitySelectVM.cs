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
        private Rectangle _Selection;

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

        private List<int> _SelectedLeds;
        public List<LedModelID> SelectedLeds
        {            
            get
            {
                List<LedModelID> res = new List<LedModelID>();
                foreach (int index in _SelectedLeds)
                {
                    res.Add(_IndexToLedID(index));
                }

                return res;
            }
            set
            {
                _SelectedLeds.Clear();
                foreach (LedModelID ID in value)
                {
                    _SelectedLeds.Add(_LedIDToIndex(ID));
                }

                _SetLedColor(_SelectedLeds, Defines.LedSelectedColor);
            }
        }

        public override EffectBaseVM CurrentEffect
        {
            get => _currentEffect;
            set
            {
                if (_currentEffect != value)
                {
                    _SetLedColor(_SelectedLeds, Defines.LedColor);
                    _currentEffect = value;

                    if (_currentEffect != null)
                        SelectedLeds = _currentEffect.SelectedLeds;
                    else
                        _SelectedLeds.Clear();

                    RaisePropertyChanged(nameof(CurrentEffect));
                    _SendMessage(MediatorMessages.LedEntitySelectVM_CurrentEffectChanged, null);
                }
            }
        }

        public LedEntitySelectVM(Model.LedEntity ledEntity)
            : base(ledEntity)
        {
            _SelectedLeds = new List<int>();

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
                foreach (var ID in _ScanForLeds())
                {
                    if (!_SelectedLeds.Contains(ID))
                        _SelectedLeds.Add(ID);
                }
            }

            _Selection = null;
            RaisePropertyChanged(nameof(FrontLedGroups));
            RaisePropertyChanged(nameof(BackLedGroups));
        }

        private void _UpdateCurrentEffect()
        {
            if (Effects.Count > 0)
                CurrentEffect = Effects.Last();
            else
                CurrentEffect = null;
        }

        private void _CreateSelectGroup(MouseEventArgs e, LedEntityView view)
        {
            Point mousePosition = e.GetPosition((IInputElement)e.Source);
            mousePosition = _ScalePoint(mousePosition);

            _Selection = new Rectangle((int)mousePosition.X, (int)mousePosition.Y, Defines.LedSelectRectangleColor);

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
                    led.Brush = Defines.LedSelectingColor;
                }
                else if (!_SelectedLeds.Contains(i))
                    led.Brush = Defines.LedColor;
            }

            return ledIndices;
        }

        private LedModelID _IndexToLedID(int index)
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

        public void AddEffect(ushort startFrame = 0, ushort endFrame = 0, EffectType effectType = EffectType.SetColor)
        {
            switch (effectType)
            {
                case EffectType.SetColor:
                    LedEntity.Effects.Add(new Model.Effect.EffectSetColor(startFrame, endFrame));
                    break;
                case EffectType.Blink:
                    LedEntity.Effects.Add(new Model.Effect.EffectBlinkColor(startFrame, endFrame));
                    break;
                case EffectType.Fade:
                    LedEntity.Effects.Add(new Model.Effect.EffectFadeColor(startFrame, endFrame));
                    break;
                case EffectType.Group:
                    break;
                default:
                    break;
            }
            
            Effects.Add(new EffectBaseVM(LedEntity, LedEntity.Effects.Last()));
            _EffectBaseVMMapping.Add(Effects.Last(), LedEntity.Effects.Last());

            _UpdateCurrentEffect();
        }

        public bool DeleteEffect(EffectBaseVM effectBaseVM)
        {
            if (_EffectBaseVMMapping.ContainsKey(effectBaseVM))
            {
                LedEntity.Effects.Remove(_EffectBaseVMMapping[effectBaseVM]);
                Effects.Remove(effectBaseVM);
                _EffectBaseVMMapping.Remove(effectBaseVM);

                _UpdateCurrentEffect();

                return true;
            }

            return false;
        }

        public bool ChangeEffectType(EffectBaseVM effectBaseVM, EffectType newEffectType)
        {
            if (_EffectBaseVMMapping.ContainsKey(effectBaseVM))            
            {
                ushort _startFrame = effectBaseVM.StartFrame;
                ushort _endFrame = effectBaseVM.EndFrame;

                LedEntity.Effects.Remove(_EffectBaseVMMapping[effectBaseVM]);
                switch (newEffectType)
                {
                    case EffectType.SetColor:                        
                        LedEntity.Effects.Add(new Model.Effect.EffectSetColor(effectBaseVM.StartFrame, effectBaseVM.EndFrame));
                        goto default;                       
                    case EffectType.Blink:
                        LedEntity.Effects.Add(new Model.Effect.EffectBlinkColor(effectBaseVM.StartFrame, effectBaseVM.EndFrame));
                        goto default;
                    case EffectType.Fade:
                        LedEntity.Effects.Add(new Model.Effect.EffectFadeColor(effectBaseVM.StartFrame, effectBaseVM.EndFrame));
                        goto default;
                    case EffectType.Group:
                        LedEntity.Effects.Add(new Model.Effect.EffectGroup(effectBaseVM.StartFrame, effectBaseVM.EndFrame));
                        goto default;
                    default:
                        _EffectBaseVMMapping[effectBaseVM] = LedEntity.Effects.Last();
                        return true;                        
                }                
            }

            return false;
        }

        public override void RecieveMessage(MediatorMessages message, object sender, object data)
        {   
            switch (message)
            {
                case MediatorMessages.EffectBaseVM_EffectTypeChanged:
                    if (sender as EffectBaseVM != CurrentEffect)
                        break;

                    MediatorMessageData.EffectBaseVM_EffectTypeChanged effectBaseVM_EffectTypeChanged = (data as MediatorMessageData.EffectBaseVM_EffectTypeChanged);

                    if (ChangeEffectType(sender as EffectBaseVM, effectBaseVM_EffectTypeChanged.NewEffectType))
                        effectBaseVM_EffectTypeChanged.SetEffect(_EffectBaseVMMapping[sender as EffectBaseVM]);
                    
                    break;
                case MediatorMessages.EffectBaseVM_EditCommand_Start:
                    if (sender as EffectBaseVM != CurrentEffect)
                        break;

                    MediatorMessageData.EffectBaseVM_EditCommand_Start effectBaseVM_EditCommand_Start = (data as MediatorMessageData.EffectBaseVM_EditCommand_Start);

                    if (effectBaseVM_EditCommand_Start.SelectedLeds != null)
                    {
                        foreach (var ID in effectBaseVM_EditCommand_Start.SelectedLeds)
                        {
                            if (!_SelectedLeds.Contains(_LedIDToIndex(ID)))
                            {
                                _SelectedLeds.Add(_LedIDToIndex(ID));
                            }
                        }
                        _SetLedColor(_SelectedLeds, Defines.LedSelectingColor);
                    }
                    _AllowSelectLeds = true;
                    break;
                case MediatorMessages.EffectBaseVM_EditCommand_Finished:
                    if (sender as EffectBaseVM != CurrentEffect)
                        break;

                    MediatorMessageData.EffectBaseVM_EditCommand_Finished effectBaseVM_EditCommand_Finished = (data as MediatorMessageData.EffectBaseVM_EditCommand_Finished);

                    _AllowSelectLeds = false;
                    _SelectingLeds = false;

                    effectBaseVM_EditCommand_Finished.SetLeds(SelectedLeds);
                    
                    _SetLedColor(_SelectedLeds, Defines.LedSelectedColor);
                    break;
                case MediatorMessages.EffectBaseVM_ClearCommand:
                    if (sender as EffectBaseVM != CurrentEffect)
                        break;

                    _SetLedColor(_SelectedLeds, Defines.LedColor);
                    _SelectedLeds.Clear();
                    break;
                case MediatorMessages.EffectBaseVM_DeleteCommand:
                    if (sender as EffectBaseVM != CurrentEffect)
                        break;

                    DeleteEffect(sender as EffectBaseVM);
                    break;
                default:
                    break;
            }
        }
    }
}
