using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Led.ViewModels
{
    public class EffectBaseVM : INPC, Interfaces.IParticipant
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
                    _SetEffectVMs();
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
                    EffectBase.EndFrame = (ushort)(value + StartFrame);

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
                    _SendMessage(MediatorMessages.EffectBaseVM_EffectTypeChanged, new MediatorMessageData.EffectBaseVM_EffectTypeChanged(value, _SetEffect_Callback));
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

        private bool _EditingLeds;
        public string EditCommandContent
        {
            get => _EditingLeds ? "Save" : "Edit";
        }
        public Command EditCommand { get; set; }
        public Command ClearCommand { get; set; }

        public Command PreviewCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command RefreshCommand { get; set; }

        public EffectBaseVM(Model.Effect.EffectBase effectBase)
        {
            SetColorVMs = new ObservableCollection<EffectProperties.SetColorVM>();
            BlinkVMs = new ObservableCollection<EffectProperties.BlinkVM>();
            FadeVMs = new ObservableCollection<EffectProperties.FadeVM>();

            EffectBase = effectBase;            

            EditCommand = new Command(OnEditCommand);
            ClearCommand = new Command(OnClearCommand);
            DeleteCommand = new Command(OnDeleteCommand);

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        private void _ClearCollections()
        {
            SetColorVMs.Clear();
            BlinkVMs.Clear();
            FadeVMs.Clear();
        }

        private void _SetEffectVMs()
        {
            _ClearCollections();

            switch (EffectType)
            {
                case EffectType.SetColor:
                    SetColorVMs.Add(new EffectProperties.SetColorVM(EffectBase as Model.Effect.EffectSetColor));
                    break;
                case EffectType.Blink:
                    BlinkVMs.Add(new EffectProperties.BlinkVM(EffectBase as Model.Effect.EffectBlinkColor));
                    break;
                case EffectType.Fade:
                    FadeVMs.Add(new EffectProperties.FadeVM(EffectBase as Model.Effect.EffectFadeColor));
                    break;
                case EffectType.Group:
                    break;
                default:
                    break;
            }
        }

        private void _SetEffect_Callback(Model.Effect.EffectBase effectBase)
        {
            EffectBase = effectBase;
        }

        private void _SetLeds_Callback(List<Utility.LedModelID> ledModelIDs)
        {
            SelectedLeds = ledModelIDs;

            EffectBase.LedPositions = App.Instance.EffectService.CalculateRelativeLedPosition(ledModelIDs, this);
        }

        public void OnEditCommand()
        {
            _EditingLeds = !_EditingLeds;
            RaisePropertyChanged(nameof(EditCommandContent));

            if (_EditingLeds)
                _SendMessage(MediatorMessages.EffectBaseVM_EditCommand_Start, new MediatorMessageData.EffectBaseVM_EditCommand_Start(_effectBase.Leds));
            else
                _SendMessage(MediatorMessages.EffectBaseVM_EditCommand_Finished, new MediatorMessageData.EffectBaseVM_EditCommand_Finished(_SetLeds_Callback));
        }

        public void OnClearCommand()
        {
            SelectedLeds.Clear();
            RaisePropertyChanged(nameof(NumberOfLeds));

            _SendMessage(MediatorMessages.EffectBaseVM_ClearCommand, null);
        }

        public void OnPreviewCommand()
        {

        }

        public void OnDeleteCommand()
        {
            _SendMessage(MediatorMessages.EffectBaseVM_DeleteCommand, null);
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
                default:
                    break;
            }
        }
    }
}
