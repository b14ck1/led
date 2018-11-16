using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    public class NetworkClientVM : INPC, Interfaces.IParticipant
    {
        public object Lock { get; }

        private Services.MediatorService _MediatorService;        

        public bool Ready { get; private set; }
        private bool _playing;

        private Services.lib.TCP.EntityMessage _LastMessageSent;
        public Services.lib.TCP.EntityMessage LastMessageSent
        {
            get => _LastMessageSent;
            set
            {
                if(_LastMessageSent != value)
                {
                    _LastMessageSent = value;
                    RaisePropertyChanged(nameof(LastMessageSent));
                }
                Ready = false;
            }
        }

        private TcpMessages _LastMessageReceived;
        public TcpMessages LastMessageReceived
        {
            get => _LastMessageReceived;
            set
            {
                if (_LastMessageReceived != value)
                {
                    _LastMessageReceived = value;
                    if(_LastMessageReceived == TcpMessages.Ready)
                    {
                        if (_LastMessageSent.TcpMessage == TcpMessages.Config)
                        {
                            ConfigSynchronized = true;
                            RaisePropertyChanged(nameof(ConfigSynchronized));
                        }
                        if (_LastMessageSent.TcpMessage == TcpMessages.Effects)
                        {
                            EffectsSynchronized = true;
                            RaisePropertyChanged(nameof(EffectsSynchronized));
                        }
                        Ready = true;
                    }
                    RaisePropertyChanged(nameof(LastMessageReceived));
                }
            }
        }

        private string _ID;
        /// <summary>
        /// ID of this network device.
        /// </summary>
        public string ID
        {
            get => _ID;
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    RaisePropertyChanged(nameof(ID));
                }
            }
        }

        private LedEntityBaseVM _SelectedEntity;
        /// <summary>
        /// LedEntity this network devices belongs to.
        /// </summary>
        public LedEntityBaseVM SelectedEntity
        {
            get => _SelectedEntity;
            set
            {
                if (_SelectedEntity != value)
                {
                    if (_SelectedEntity != null)
                        _SelectedEntity.ClientID = String.Empty;

                    _SelectedEntity = value;
                    if (_SelectedEntity != null)
                        _SelectedEntity.ClientID = ID;

                    RaisePropertyChanged(nameof(SelectedEntity));
                    RaisePropertyChanged(nameof(SelectableEntities));

                    _MediatorService.BroadcastMessage(MediatorMessages.NetworkClient_BindingChanged, this, null);
                }
            }
        }
        
        /// <summary>
        /// LedEntities which don't belong to a device yet.
        /// </summary>
        public List<LedEntityBaseVM> SelectableEntities
        {
            get
            {
                List<LedEntityBaseVM> res = new List<LedEntityBaseVM>();
                foreach (var x in App.Instance.MainWindowVM.LedEntities)
                {
                    if (x.ClientID.Equals(""))
                        res.Add(x);
                }
                return res;
            }
        }

        private bool _ConfigSynchronized;
        /// <summary>
        /// Is the config on this device synchronized.
        /// Gets set to false when the corresponding LedEntity gets edited.
        /// Gets set to true when the config is sent and we received a ConfigReady.
        /// </summary>
        public bool ConfigSynchronized
        {
            get => _ConfigSynchronized;
            set
            {
                if(_ConfigSynchronized != value)
                {
                    _ConfigSynchronized = value;
                    RaisePropertyChanged(nameof(ConfigSynchronized));
                }
            }
        }

        private bool _EffectsSynchronized;
        /// <summary>
        /// Are the effects on this device synchronized.
        /// Gets set to false when one of the corresponding Effects gets edited.
        /// Gets set to true when the effects are sent and we received a EffectsReady.
        /// </summary>
        public bool EffectsSynchronized
        {
            get => _EffectsSynchronized;
            set
            {
                if (_EffectsSynchronized != value)
                {
                    _EffectsSynchronized = value;
                    RaisePropertyChanged(nameof(EffectsSynchronized));
                }
            }
        }

        public Command ShowClientCommand { get; set; }
        public Command RemoveBindingCommand { get; set; }

        public Command RestartCommand { get; set; }
        public Command ShutdownCommand { get; set; }
        
        /// <summary>
        /// Remap LedEntity to match our ID.
        /// </summary>
        public void Remap()
        {
            foreach(var x in App.Instance.MainWindowVM.LedEntities)
            {
                if(x.ClientID == ID)
                {
                    _SelectedEntity = x;
                    return;
                }
            }

            _SelectedEntity = null;
        }

        public NetworkClientVM(string id)
        {
            Lock = new object();            

            ID = id;

            ShowClientCommand = new Command(_OnShowClientCommand, () => !_playing);
            RemoveBindingCommand = new Command(_OnRemoveBindingCommand, () => !_playing);

            RestartCommand = new Command(_OnRestartCommand, () => !_playing);
            ShutdownCommand = new Command(_OnShutdownCommand, () => !_playing);

            _MediatorService = App.Instance.MediatorService;
            _MediatorService.Register(this);
        }

        private void _RaiseCommandsChanged()
        {
            ShowClientCommand.RaiseCanExecuteChanged();
            RemoveBindingCommand.RaiseCanExecuteChanged();

            RestartCommand.RaiseCanExecuteChanged();
            ShutdownCommand.RaiseCanExecuteChanged();
        }

        private void _OnShowClientCommand()
        {
            App.Instance.ConnectivityService.SendMessage(TcpMessages.Show, ID);
        }

        private void _OnRestartCommand()
        {
            Views.Dialogs.YesNoDialog yesNoDialog = new Views.Dialogs.YesNoDialog();
            ViewModels.YesNoDialogVM yesNoDialogVM = new YesNoDialogVM("Bitte bestätigen", "Gerät neustarten.");
            App.Instance.WindowService.ShowNewWindow(yesNoDialog, yesNoDialogVM);

            if(yesNoDialogVM.DialogResult)
                App.Instance.ConnectivityService.SendMessage(TcpMessages.Restart, ID);

        }

        private void _OnShutdownCommand()
        {
            Views.Dialogs.YesNoDialog yesNoDialog = new Views.Dialogs.YesNoDialog();
            ViewModels.YesNoDialogVM yesNoDialogVM = new YesNoDialogVM("Bitte bestätigen", "Gerät ausschalten.");
            App.Instance.WindowService.ShowNewWindow(yesNoDialog, yesNoDialogVM);

            if (yesNoDialogVM.DialogResult)
                App.Instance.ConnectivityService.SendMessage(TcpMessages.Shutdown, ID);
        }

        private void _OnRemoveBindingCommand()
        {
            SelectedEntity = null;
            _MediatorService.BroadcastMessage(MediatorMessages.NetworkClient_BindingChanged, this, null);
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.NetworkClient_BindingChanged:
                    RaisePropertyChanged(nameof(SelectableEntities));
                    break;
                case MediatorMessages.AudioControlPlayPause:
                    _playing = (data as MediatorMessageData.AudioControlPlayPauseData).Playing;
                    _RaiseCommandsChanged();
                    break;
                case MediatorMessages.LedEntityCRUDVM_Editing:
                    if ((data as MediatorMessageData.LedEntityCRUDVM_Editing).ID.Equals(ID))
                        ConfigSynchronized = false;
                    break;
                case MediatorMessages.EffectBaseVM_EffectChanged:
                    if ((data as MediatorMessageData.EffectBaseVM_EffectChanged).ID.Equals(ID))
                        EffectsSynchronized = false;
                    break;
            }
        }
    }
}
