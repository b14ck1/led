using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels.EffectProperties
{
    public class SetColorVM : BaseVM
    {
        public SetColorVM(Model.Effect.EffectSetColor effectSetColor)
            :base(effectSetColor)
        {

        }
    }
}