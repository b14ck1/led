using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Led.ViewModels
{
    class EffectBaseVM : INPC, Interfaces.IParticipant
    {
        private Services.MediatorService _Mediator;

        private Model.Effect.EffectBase _effectBase;
        public Model.Effect.EffectBase EffectBase
        {
            get => _effectBase;
            set
            {
                if (_effectBase != value)
                {
                    _effectBase = value;
                    RaiseAllPropertyChanged();
                }
            }
        }

        public bool EffectActive
        {
            get => _effectBase.Active;
            set
            {
                if (_effectBase.Active != value)
                {
                    _effectBase.Active = value;
                    RaisePropertyChanged(nameof(EffectActive));
                }
            }
        }

        public short StartFrame
        {
            get => _effectBase.StartFrame;
            set
            {
                if (_effectBase.StartFrame != value && value >= 0)
                {
                    _effectBase.StartFrame = value;
                    _effectBase.EndFrame = (short)(StartFrame + Dauer);
                    RaisePropertyChanged(nameof(StartFrame));
                    RaisePropertyChanged(nameof(Endframe));
                }
            }
        }
        public short Dauer
        {
            get => (short)(_effectBase.EndFrame - _effectBase.StartFrame);
            set
            {
                if (Dauer != value && value > 0)
                {
                    _effectBase.EndFrame = (short)(StartFrame + Dauer);
                    RaisePropertyChanged(nameof(Dauer));
                    RaisePropertyChanged(nameof(Endframe));
                }
            }
        }
        public short Endframe
        {
            get => _effectBase.EndFrame;
            set
            {
                if (_effectBase.EndFrame != value && value > StartFrame)
                {
                    _effectBase.EndFrame = value;                    
                    RaisePropertyChanged(nameof(Endframe));
                    RaisePropertyChanged(nameof(Dauer));
                }
            }
        }

        public EffectType EffectType
        {
            get => _effectBase.EffectType;
            set
            {
                    if (_effectBase.EffectType != value)
                {
                    switch (value)
                    {
                        case EffectType.SetColor:
                            _effectBase = new Model.Effect.EffectSetColor();
                            break;
                        case EffectType.Blink:
                            _effectBase = new Model.Effect.EffectBlinkColor();
                            break;
                        case EffectType.Fade:
                            _effectBase = new Model.Effect.EffectFadeColor();
                            break;
                        case EffectType.Group:
                            _effectBase = new Model.Effect.EffectGroup();
                            break;
                        default:
                            break;
                    }

                    RaisePropertyChanged(nameof(EffectType));
                }
            }
        }

        public List<Utility.LedModelID> Leds
        {
            get => _effectBase.Leds;
            set
            {
                _effectBase.Leds = value;
                RaisePropertyChanged(nameof(NumLEDs));
            }
        }
        public int NumLEDs
        {
            get => _effectBase.Leds.Count();
        }

        private bool _editingLeds;
        public string EditCommandContent
        {
            get => _editingLeds ? "Save" : "Edit";
        }
        public Command EditCommand { get; set; }
        public Command ClearCommand { get; set; }

        public Command PreviewCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command RefreshCommand { get; set; }

        public static string OnLedEditCommandMessage = "LedEditCommand";

        public EffectBaseVM()
        {
            EffectBase = new Model.Effect.EffectSetColor();

            EditCommand = new Command(OnEditCommand);

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        public EffectBaseVM(Model.Effect.EffectBase EffectBase)        
        {
            this.EffectBase = EffectBase;
        }

        public void OnEditCommand()
        {
            if (_editingLeds)
            {
                _editingLeds = false;
                RaisePropertyChanged(nameof(EditCommandContent));
                SendMessage(MediatorMessages.EditedSelectedLeds, new EditLedArgs(false, _effectBase.Leds));                
            }
            else
            {
                _editingLeds = true;
                RaisePropertyChanged(nameof(EditCommandContent));
                SendMessage(MediatorMessages.EditedSelectedLeds, new EditLedArgs(true, _effectBase.Leds));                
            }
        }

        public void OnClearCommand()
        {
            
        }

        public void OnPreviewCommand()
        {

        }

        public void OnDeleteCommand()
        {

        }

        public void OnRefreshCommand()
        {

        }

        protected void SendMessage(MediatorMessages message, object data)
        {
            _Mediator.BroadcastMessage(message, this, data);
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            //throw new NotImplementedException();
        }
    }

    public class EditLedArgs
    {
        public bool Edit;
        public List<Utility.LedModelID> SelectedLeds;

        public EditLedArgs(bool Edit, List<Utility.LedModelID> SelectedLeds)
        {
            this.Edit = Edit;
            this.SelectedLeds = SelectedLeds;
        }
    }
}
