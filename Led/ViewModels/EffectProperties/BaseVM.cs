using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels.EffectProperties
{
    public class BaseVM : INPC
    {
        private Model.Effect.EffectBase _EffectBase;

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

        public Command AddColorCommand { get; set; }
        
        public BaseVM(Model.Effect.EffectBase effectBase)
        {
            _EffectBase = effectBase;
            Colors = new ObservableCollection<EffectColorVM>();
            for (int i = 0; i < effectBase.Colors.Count; i++)
            {
                Colors.Add(new EffectColorVM(effectBase, i));
                Colors.Last().OnRemove += _OnRemoveColor;
            }

            AddColorCommand = new Command(_OnAddColor);
        }

        private void _OnAddColor()
        {
            _EffectBase.Colors.Add(System.Windows.Media.Colors.Transparent);
            Colors.Add(new EffectColorVM(_EffectBase, Colors.Count));
            Colors.Last().OnRemove += _OnRemoveColor;
        }

        private void _OnRemoveColor(object sender, EventArgs e)
        {
            _EffectBase.Colors.RemoveAt(Colors.IndexOf((sender as EffectColorVM)));
            _colors.Remove((sender as EffectColorVM));
            (sender as EffectColorVM).OnRemove -= _OnRemoveColor;
            for(int i = 0; i < Colors.Count; i++)
            {
                Colors[i].Index = i;
            }
        }
    }
}
