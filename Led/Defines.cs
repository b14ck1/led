using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

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

    public class LedGroupIdentifier
    {
        public byte BusID;
        public byte PositionInBus;

        public LedGroupIdentifier(byte busID, byte positionInBus)
        {
            BusID = busID;
            PositionInBus = positionInBus;
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
        LedEntitySelectButtonClicked,
        EffectBaseVM_EffectTypeChanged,
        EffectBaseVM_EditCommand_Start,
        EffectBaseVM_EditCommand_Finished,
        EffectBaseVM_ClearCommand,
        EffectBaseVM_DeleteCommand,
        LedEntitySelectVM_CurrentEffectChanged,
        LedEntityCRUDVM_GroupBusDefinitionsChanged,
        LedEntityCRUDVM_GroupBusDefinitionsNeedCorrectionChanged,
        LedEntityCRUDVM_GroupPositionChanged,
        LedEntityCRUDVM_GroupPositionNeedCorrectionChanged,
        TimeLineCollectionChanged,
        TimeLineEffectSelected,
        AudioControlPlayPause,
        AudioControlCurrentTick,
        AudioProperty_NewAudio,
        EffectService_RenderAll,
        EffectService_Preview,
        EffectService_AskCurrentLedEntities,
        EffectService_RecieveCurrentLedEntities,
        TcpServer_ClientsChanged,
        NetworkClient_BindingChanged
    }

    public class MediatorMessageData
    {
        public class EffectBaseVM_EffectTypeChanged
        {
            public EffectType NewEffectType { get; }

            public Action<Model.Effect.EffectBase> SetEffect;

            public EffectBaseVM_EffectTypeChanged(EffectType newEffectType, Action<Model.Effect.EffectBase> action)
            {
                NewEffectType = newEffectType;
                SetEffect = action;
            }
        }

        public class EffectBaseVM_EditCommand_Start
        {
            public List<Utility.LedModelID> SelectedLeds { get; }

            public EffectBaseVM_EditCommand_Start(List<Utility.LedModelID> selectedLeds)
            {
                SelectedLeds = selectedLeds;
            }
        }

        public class EffectBaseVM_EditCommand_Finished
        {
            public Action<List<Utility.LedModelID>> SetLeds;

            public EffectBaseVM_EditCommand_Finished(Action<List<Utility.LedModelID>> action)
            {
                SetLeds = action;
            }
        }

        public class LedEntityCRUDVM_GroupBusDefinitionsNeedCorrectionChanged
        {
            public Dictionary<ViewModels.LedGroupPropertiesVM, bool> NeedCorrection { get; }
            
            public LedEntityCRUDVM_GroupBusDefinitionsNeedCorrectionChanged(Dictionary<ViewModels.LedGroupPropertiesVM, bool> needCorrection)
            {
                NeedCorrection = needCorrection;
            }
        }

        public class LedEntityCRUDVM_GroupPositionNeedCorrectionChanged
        {
            public Dictionary<ViewModels.LedGroupPropertiesVM, bool> NeedCorrection { get; }

            public LedEntityCRUDVM_GroupPositionNeedCorrectionChanged(Dictionary<ViewModels.LedGroupPropertiesVM, bool> needCorrection)
            {
                NeedCorrection = needCorrection;
            }
        }

        public class TimeLineCollectionChangedData
        {
            public ObservableCollection<ViewModels.EffectBaseVM> Effects { get; }

            public TimeLineCollectionChangedData(ObservableCollection<ViewModels.EffectBaseVM> effects)
            {
                Effects = effects;
            }
        }

        public class TimeLineEffectSelectedData
        {
            public ViewModels.EffectBaseVM EffectBaseVM { get; }
            
            public TimeLineEffectSelectedData(ViewModels.EffectBaseVM effectBaseVM)
            {
                EffectBaseVM = effectBaseVM;
            }
        }

        public class AudioControlPlayPauseData
        {
            public long CurrentFrame { get; }
            public bool Playing { get; }

            public AudioControlPlayPauseData(long currentFrame, bool playing)
            {
                CurrentFrame = currentFrame;
                Playing = playing;
            }
        }

        public class AudioControlCurrentFrameData
        {
            public long CurrentFrame { get; }

            public AudioControlCurrentFrameData(long currentFrame)
            {
                CurrentFrame = currentFrame;
            }
        }

        public class AudioProperty_NewAudio
        {
            public Model.AudioProperty AudioProperty { get; }

            public AudioProperty_NewAudio(Model.AudioProperty audioProperty)
            {
                AudioProperty = audioProperty;
            }
        }

        public class EffectServicePreview
        {
            public ViewModels.EffectBaseVM EffectBaseVM { get; }

            public bool Stop { get; }

            public EffectServicePreview(ViewModels.EffectBaseVM effectBaseVM, bool stop = false)
            {
                EffectBaseVM = effectBaseVM;
                Stop = stop;
            }
        }

        public class EffectService_RecieveCurrentLedEntities
        {
            public ObservableCollection<ViewModels.LedEntityBaseVM> LedEntityBaseVMs { get; }

            public EffectService_RecieveCurrentLedEntities(ObservableCollection<ViewModels.LedEntityBaseVM> ledEntityBaseVMs)
            {
                LedEntityBaseVMs = ledEntityBaseVMs;
            }
        }

        //public class TimeLineEffectPropertiesChangedData
        //{
        //    public enum Values
        //    {
        //        StartFrame,
        //        Dauer,
        //        EndFrame
        //    }

        //    public Values ChangedValue { get; }
        //    public ushort Value { get; }

        //    public TimeLineEffectPropertiesChangedData(Values changedValue, ushort value)
        //    {
        //        ChangedValue = changedValue;
        //        Value = value;
        //    }
        //}
    }

    public enum TcpMessages
    {
        ID,
        Config,
        RenderedEffects,
        Timestamp,
        Play,
        Pause,
        Preview,
        Show
    }

    public static class Defines
    {
        public static Brush LedGroupColor = Brushes.DarkSlateGray;
        public static Brush LedGroupColorWrong = Brushes.Red;
        public static Brush LedSelectRectangleColor = Brushes.Red;

        public static Brush LedColor = Brushes.LimeGreen;
        public static Brush LedSelectingColor = Brushes.Blue;
        public static Brush LedSelectedColor = Brushes.Red;

        public static int MainWindowWidth = 1600;
        public static int MainWindowHeight = 900;

        public static byte FramesPerSecond = 40;

        public const int ServerPort = 31313;        
        public const string UdpBroadcastMessage = "I bims, eins LED!";
        public const string UdpBroadcastAnswer = "I bims, eins PC!";        
    }
}
