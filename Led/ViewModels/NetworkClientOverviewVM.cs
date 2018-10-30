using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    class NetworkClientOverviewVM : INPC, Interfaces.IParticipant
    {
        private Services.MediatorService _MediatorService;

        private ObservableCollection<LedEntityBaseVM> _LedEntityBaseVMs;

        public ObservableCollection<NetworkClientVM> NetworkClientVMs { get; set; }

        public NetworkClientOverviewVM(ObservableCollection<LedEntityBaseVM> ledEntityBaseVMs)
        {
            _LedEntityBaseVMs = ledEntityBaseVMs;
            NetworkClientVMs = new ObservableCollection<NetworkClientVM>();

            _MediatorService = App.Instance.MediatorService;
            _MediatorService.Register(this);
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.TcpServer_ClientsChanged:
                    List<string> _ids = new List<string>(App.Instance.ConnectivityService.ConnectedClients);
                    foreach(var client in NetworkClientVMs.ToList())
                    {
                        if (!_ids.Contains(client.ID))
                            NetworkClientVMs.Remove(client);
                    }
                    foreach(var id in _ids)
                    {
                        if (NetworkClientVMs.Select(x => x.ID.Equals(id)).ToList().Count == 0)
                            NetworkClientVMs.Add(new NetworkClientVM(id, _LedEntityBaseVMs));
                    }
                    break;
            }
        }
    }
}
