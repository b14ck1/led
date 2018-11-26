using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Led.ViewModels.EffectProperties
{
    public class EffectColorVM : INPC
    {
        private Model.Effect.EffectBase _EffectBase { get; }

        public Color Color
        {
            get => _EffectBase.Colors[Index];
            set
            {
                if (_EffectBase.Colors[Index] != value)
                {
                    _EffectBase.Colors[Index] = value;
                    RaisePropertyChanged(nameof(ColorAsBrush));
                }
            }
        }
        public Brush ColorAsBrush => new SolidColorBrush(Color);

        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    RaisePropertyChanged(nameof(Index));
                }
            }
        }

        public Command PickColorCommand { get; set; }
        public Command RemoveCommand { get; set; }

        public event EventHandler OnRemove;

        public EffectColorVM(Model.Effect.EffectBase effectBase, int index)
        {
            _EffectBase = effectBase;
            Index = index;

            PickColorCommand = new Command(_OnPickColorCommand);
            RemoveCommand = new Command(_OnRemoveCommand);
        }

        private void _OnPickColorCommand()
        {
            ColorPickerVM colorPickerVM = new ColorPickerVM(Color, App.Instance.ProjectService.GetLedEntity(_EffectBase));
            App.Instance.WindowService.ShowNewWindow(new Views.Controls.ColorPicker(), colorPickerVM);
            Color = Color.FromArgb(colorPickerVM.CurrColor.AScaled, colorPickerVM.CurrColor.R, colorPickerVM.CurrColor.G, colorPickerVM.CurrColor.B);
        }

        private void _OnRemoveCommand()
        {
            OnRemove?.Invoke(this, null);
        }
    }
}