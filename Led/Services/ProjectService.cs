using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Services
{
    public class ProjectService
    {
        public Model.LedEntity GetLedEntity(Model.Effect.EffectBase effectBase)
        {
            foreach (var ledEntity in App.Instance.MainWindowVM.Project.LedEntities)
            {
                foreach (var eBase in ledEntity.Effects)
                {
                    if (eBase == effectBase)
                        return ledEntity;
                }
            }

            return null;
        }
    }
}
