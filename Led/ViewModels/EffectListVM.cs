using Led.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    class EffectListVM : INPC, IParticipant
    {
        private Services.MediatorService _Mediator;

        public ObservableCollection<EffectBaseVM> EffectList { get; set; }

        public EffectListVM()
        {
            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {            
            switch (message)
            {
                case MediatorMessages.TimeLineCollectionChanged:
                    EffectList = null;
                    EffectList = (data as MediatorMessageData.TimeLineCollectionChangedData).Effects;
                    RaisePropertyChanged(nameof(EffectList));
                    break;
            }
        }
    }
}
