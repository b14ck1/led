using Led.Interfaces;
using Led.Model.Effect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    class TimeLineUserControlVM : IParticipant
    {
        private Services.MediatorService _Mediator;

        private ObservableCollection<EffectBaseVM> effects;

        public TimeLineUserControlVM()
        {
            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            Debug.WriteLine("TUCVM received message: " + message);
            switch (message)
            {
                case MediatorMessages.TimeLineCollectionChanged:
                    effects = (data as MediatorMessageData.TimeLineCollectionChangedData).Effects;
                    if (Debugger.IsAttached)
                    {
                        if (effects.Count > 0)
                        {
                            foreach (EffectBaseVM vm in effects)
                            {
                                Debug.WriteLine("    " + vm);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("    empty effects");
                        }
                    }
                    break;
            }
        }
    }

}
