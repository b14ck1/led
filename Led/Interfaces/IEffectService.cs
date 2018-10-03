using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led
{
    interface IEffectService
    {
        void RegisterEntity(ViewModels.LedEntityBaseVM ledEntity);        
    }
}
