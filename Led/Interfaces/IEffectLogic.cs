using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led
{
    public interface IEffectLogic
    {
        bool Active { get; }

        ushort StartFrame { get; }

        ushort Dauer { get; }

        ushort EndFrame { get; }

        List<Model.LedChangeData> LedChangeDatas { get; }
    }
}
