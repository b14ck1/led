using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace Led
{
    public class INPC : System.ComponentModel.INotifyPropertyChanged
    {
        protected void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (string propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        protected void RaisePropertyChanged(IList<string> propertyNames)
        {
            foreach (string propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        protected void RaiseAllPropertyChanged()
        {
            RaisePropertyChanged(String.Empty);
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

    public class Command : ICommand
    {
        Action _TargetExecuteMethod;
        Func<bool> _TargetCanExecuteMethod;

        public Command(Action executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public Command(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {

            if (_TargetCanExecuteMethod != null)
            {
                return _TargetCanExecuteMethod();
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            _TargetExecuteMethod?.Invoke();
        }
    }

    public class Command<T> : ICommand
    {
        Action<T> _TargetExecuteMethod;
        Func<bool> _TargetCanExecuteMethod;

        public Command(Action<T> executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public Command(Action<T> executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        bool ICommand.CanExecute(object parameter)
        {

            if (_TargetCanExecuteMethod != null)
            {
                return _TargetCanExecuteMethod();
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        void ICommand.Execute(object parameter)
        {
            _TargetExecuteMethod?.Invoke((T)parameter);
        }
    }

    class LedGroupIdentifier
    {
        public byte BusID;
        public byte PositionInBus;

        public LedGroupIdentifier(byte BusID, byte PositionInBus)
        {
            this.BusID = BusID;
            this.PositionInBus = PositionInBus;
        }

        public override int GetHashCode()
        {
            int res = BusID;
            res = res << 8;
            res += PositionInBus;

            return res.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return BusID == (obj as LedGroupIdentifier).BusID && PositionInBus == (obj as LedGroupIdentifier).PositionInBus;
        }
    }

    public enum LedEntityView
    {
        Front,
        Back
    }

    public enum LedViewArrowDirection
    {
        Up,
        Right,
        Down,
        Left,
        None
    }

    public enum EffectType
    {
        SetColor,
        Blink,
        Fade,
        Group
    }

    public enum MediatorMessages
    {
        LedEntitySelected,
        EditedSelectedLeds
    }

    public static class Defines
    {
        public static Brush LedGroupColor = Brushes.DarkSlateGray;
        public static Brush LedGroupColorWrong = Brushes.Red;
        public static Brush LedSelectRectangleColor = Brushes.Red;

        public static Brush LedColor = Brushes.LimeGreen;
        public static Brush LedSelectedColor = Brushes.Blue;

        public static int MainWindowWidth = 1600;
        public static int MainWindowHeight = 900;
    }
}
