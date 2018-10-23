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
    class TimelineUserControlVM : INPC,IParticipant
    {
        private Services.MediatorService _Mediator;

        public ObservableCollection<EffectBaseVM> Effects { get; set; }
        public ushort TotalFrames { get; set; } // TODO item can be moved passed this value if you move your mouse fast...

        public TimelineUserControlVM()
        {
            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);

            TotalFrames = 5000;

            Effects = new ObservableCollection<EffectBaseVM>();

            //var tmp1 = new EffectBaseVM(new EffectBlinkColor()
            //{
            //    StartFrame = 3,
            //    EndFrame = 18,
            //    //Name = "Temp 1"
            //});
            //var tmp2 = new EffectBaseVM(new EffectFadeColor()
            //{
            //    StartFrame = 18,
            //    EndFrame = 33,
            //    //Name = "Temp 2"
            //});

            //Effects.Add(tmp1);
            //Effects.Add(tmp2);
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            //Debug.WriteLine("TUCVM received message: " + message);
            switch (message)
            {
                case MediatorMessages.TimeLineCollectionChanged:
                    // sort, TimeLineControl needs the lowest one to be first etc.
                    Effects = (data as MediatorMessageData.TimeLineCollectionChangedData).Effects;
                    RaisePropertyChanged(nameof(Effects));
                    break;
            }
        }
    }

}
