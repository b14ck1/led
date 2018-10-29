using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    class NetworkClientOverviewVM
    {
        public ObservableCollection<NetworkClientVM> NetworkClientVMs { get; set; }
    }
}
