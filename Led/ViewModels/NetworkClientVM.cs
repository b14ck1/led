using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    class NetworkClientVM : INPC, Interfaces.IParticipant
    {
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

        private ViewModels.LedEntityBaseVM _LedEntityBaseVM;
        string Entity => _LedEntityBaseVM.Name;

        public Command ChangeEntityCommand { get; set; }

        public NetworkClientVM(string id)
        {
            ID = id;
            ChangeEntityCommand = new Command(_OnChangeEntityCommand);

            _MediatorService = App.Instance.MediatorService;
            _MediatorService.Register(this);
        }

        private void _OnChangeEntityCommand()
        {
            //Request List mit aktuellen offenen Entities
            //Ref auf Liste mit aktuellen Clients?
            //Einen auswählen
            //ID wird in VM geschrieben, somit auch ins Model
            //Oder dementsprechen gelöscht
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            throw new NotImplementedException();
        }
    }
}
