using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels.EffectProperties
{
    public class SetColorVM : INPC
    {
        private ObservableCollection<EffectColorVM> _colors;
        public ObservableCollection<EffectColorVM> Colors
        {
            get => _colors;
            set
            {
                if (_colors != value)
                {
                    _colors = value;
                    RaisePropertyChanged(nameof(Colors));
                }
            }
        }

        public SetColorVM(Model.Effect.EffectSetColor effectSetColor)
        {
            Colors = new ObservableCollection<EffectColorVM>();
            for (int i = 0; i < effectSetColor.Colors.Count; i++)
            {
                Colors.Add(new EffectColorVM(effectSetColor as Model.Effect.EffectBase, i));
            }
        }
    }
    }
