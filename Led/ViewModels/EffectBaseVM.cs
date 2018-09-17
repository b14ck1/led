using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            get => EffectBase.Active;
            set
            {
                if (EffectBase.Active != value)
                {
                    EffectBase.Active = value;
                    RaisePropertyChanged(nameof(EffectActive));
                }
            }
        }

        public ushort StartFrame
        {
            get => EffectBase.StartFrame;
            set
            {
                if (EffectBase.StartFrame != value)
                {
                    EffectBase.EndFrame = (ushort)(value + Dauer);
                    EffectBase.StartFrame = value;
                    
                    RaisePropertyChanged(nameof(StartFrame));
                    RaisePropertyChanged(nameof(EndFrame));
                }
            }
        }
        public ushort Dauer
        {
            get => EffectBase.Dauer;
            set
            {
                if (Dauer != value)
                {
                    EffectBase.EndFrame = (ushort)(StartFrame + value);

                    RaisePropertyChanged(nameof(Dauer));
                    RaisePropertyChanged(nameof(EndFrame));
                }
            }
        }
        public ushort EndFrame
        {
            get => EffectBase.EndFrame;
            set
            {
                if (EffectBase.EndFrame != value && value >= StartFrame)
                {
                    EffectBase.EndFrame = value;                    
                    RaisePropertyChanged(nameof(EndFrame));
                    RaisePropertyChanged(nameof(Dauer));
                }
            }
        }

        public EffectType EffectType
        {
            get => EffectBase.EffectType;
            set
            {
                if (EffectBase.EffectType != value)
                {
                    switch (value)
                    {
                        case EffectType.SetColor:
                            BlinkVMs.Clear();
                            FadeVMs.Clear();
                            SetColorVMs.Add(new EffectProperties.SetColorVM());                                                        
                            EffectBase = new Model.Effect.EffectSetColor(StartFrame, EndFrame);
                            break;
                        case EffectType.Blink:
                            SetColorVMs.Clear();
                            FadeVMs.Clear();
                            BlinkVMs.Add(new EffectProperties.BlinkVM());                            
                            EffectBase = new Model.Effect.EffectBlinkColor(StartFrame, EndFrame);
                            break;
                        case EffectType.Fade:
                            SetColorVMs.Clear();
                            BlinkVMs.Clear();
                            FadeVMs.Add(new EffectProperties.FadeVM());                            
                            EffectBase = new Model.Effect.EffectFadeColor(StartFrame, EndFrame);
                            break;
                        case EffectType.Group:
                            //EffectBase = new Model.Effect.EffectGroup(StartFrame, EndFrame);
                            break;
                        default:
                            break;
                    }

                    RaisePropertyChanged(nameof(EffectType));
                }
            }
        }

        private ObservableCollection<EffectProperties.SetColorVM> _setColorVMs;
        public ObservableCollection<EffectProperties.SetColorVM> SetColorVMs
        {
            get => _setColorVMs;
            set
            {
                if (_setColorVMs != value)
                {
                    _setColorVMs = value;
                    RaisePropertyChanged(nameof(SetColorVMs));
                }
            }
        }

        private ObservableCollection<EffectProperties.BlinkVM> _blinkVMs;
        public ObservableCollection<EffectProperties.BlinkVM> BlinkVMs
        {
            get => _blinkVMs;
            set
            {
                if (_blinkVMs != value)
                {
                    _blinkVMs = value;
                    RaisePropertyChanged(nameof(BlinkVMs));
                }
            }
        }

        private ObservableCollection<EffectProperties.FadeVM> _fadeVMs;
        public ObservableCollection<EffectProperties.FadeVM> FadeVMs
        {
            get => _fadeVMs;
            set
            {
                if (_fadeVMs != value)
                {
                    _fadeVMs = value;
                    RaisePropertyChanged(nameof(FadeVMs));
                }
            }
        }

        public List<Utility.LedModelID> SelectedLeds
        {
            get => EffectBase.Leds;
            set
            {
                EffectBase.Leds = value;
                RaisePropertyChanged(nameof(NumberOfLeds));
            }
        }
        public int NumberOfLeds
        {
            get => EffectBase.Leds.Count();
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
            ClearCommand = new Command(OnClearCommand);

            SetColorVMs = new ObservableCollection<EffectProperties.SetColorVM>()
            {
                new EffectProperties.SetColorVM(),
                
            };
            BlinkVMs = new ObservableCollection<EffectProperties.BlinkVM>();
            FadeVMs = new ObservableCollection<EffectProperties.FadeVM>();

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        public EffectBaseVM(Model.Effect.EffectBase EffectBase)        
        {
            this.EffectBase = EffectBase;
        }

        public void OnEditCommand()
        {
            _editingLeds = !_editingLeds;
            RaisePropertyChanged(nameof(EditCommandContent));
            _SendMessage(MediatorMessages.EffectVMEditSelectedLedsClicked, new MediatorMessageData.EffectVMEditSelectedLeds(_editingLeds, _effectBase.Leds));
        }

        public void OnClearCommand()
        {
            SelectedLeds.Clear();
            RaisePropertyChanged(nameof(NumberOfLeds));
            _SendMessage(MediatorMessages.EffectVMEditSelectedLedsFinished, null);
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

        protected void _SendMessage(MediatorMessages message, object data)
        {
            _Mediator.BroadcastMessage(message, this, data);
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.EffectVMEditSelectedLedsFinished:
                    SelectedLeds = (data as MediatorMessageData.EffectVMEditSelectedLeds).SelectedLeds;
                    break;
                case MediatorMessages.GroupBusDefinitionsChanged:
                    break;
                case MediatorMessages.GroupBusDefinitionsNeedCorrectionChanged:
                    break;
                default:
                    break;
            }
        }
    }
}
