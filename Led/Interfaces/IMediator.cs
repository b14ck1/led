using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Interfaces
{
    interface IMediator
    {
        void Register(IParticipant sender);

        void Unregister(IParticipant sender);

        void BroadcastMessage(MediatorMessages message, object sender, object data);
    }
}
