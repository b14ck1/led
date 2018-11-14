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

        public ObservableCollection<NetworkClientVM> NetworkClientVMs { get; set; }

        public NetworkClientOverviewVM()
        {
            NetworkClientVMs = new ObservableCollection<NetworkClientVM>();

            _MediatorService = App.Instance.MediatorService;
            _MediatorService.Register(this);
        }

        public void RemapClients()
        {
            foreach(var x in NetworkClientVMs)
            {
                x.Remap();
            }
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.TcpServer_NetworkClientAdded:
                    NetworkClientVMs.Add((data as NetworkClientVM));                    
                    break;
                case MediatorMessages.TcpServer_NetworkClientDropped:
                    NetworkClientVMs.Remove((data as NetworkClientVM));
                    break;
            }
        }
    }
}
