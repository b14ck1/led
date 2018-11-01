using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    class NetworkClientVM : INPC, Interfaces.IParticipant
    {
        private ObservableCollection<LedEntityBaseVM> _LedEntityBaseVMs;
        private Services.MediatorService _MediatorService;

        private string _ID;
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
        
        public List<LedEntityBaseVM> SelectableEntities
        {
            get
            {
                List<LedEntityBaseVM> res = new List<LedEntityBaseVM>();
                foreach (var x in _LedEntityBaseVMs)
                {
                    if (x.ClientID.Equals(""))
                        res.Add(x);
                }
                return res;
            }
        }

        public Command ShowClientCommand { get; set; }
        public Command RemoveBindingCommand { get; set; }

        public NetworkClientVM(string id, ObservableCollection<LedEntityBaseVM> ledEntityBaseVMs)
        {
            _LedEntityBaseVMs = ledEntityBaseVMs;

            ID = id;
            ShowClientCommand = new Command(_OnShowClientCommand);
            RemoveBindingCommand = new Command(_OnRemoveBindingCommand);

            _MediatorService = App.Instance.MediatorService;
            _MediatorService.Register(this);
        }

        private void _OnShowClientCommand()
        {
            App.Instance.ConnectivityService.SendShow(ID);
        }

        private void _OnRemoveBindingCommand()
        {
            SelectedEntity = null;
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.NetworkClient_BindingChanged:
                    RaisePropertyChanged(nameof(SelectableEntities));
                    break;
            }
        }
    }
}
