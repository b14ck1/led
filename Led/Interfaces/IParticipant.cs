using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Interfaces
{
    public interface IParticipant
    {
        void RecieveMessage(MediatorMessages message, object sender, object data);
    }
}
