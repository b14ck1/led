using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Led.Utility;

namespace Led.ViewModels
{
    public abstract class LedEntityBaseVM : INPC, Interfaces.IParticipant
    {
        private Services.MediatorService _Mediator;

        private Model.LedEntity _ledEntity;
        public Model.LedEntity LedEntity
        {
            get => _ledEntity;
            set
            {
                if (_ledEntity != value)
                {
                    _ledEntity = value;                    
                    RaiseAllPropertyChanged();
                }
            }
        }        

        /// <summary>
        /// Just to display the Name of the Entity.
        /// </summary>
        public string Name
        {
            get => _ledEntity.LedEntityName;
            set
            {
                if (_ledEntity.LedEntityName != value)
                {
                    _ledEntity.LedEntityName = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Path to the Front Image.
        /// </summary>
        public string FrontImagePath
        {
            get => _ledEntity.ImageInfos[LedEntityView.Front].Path;
            set
            {
                _ledEntity.ImageInfos[LedEntityView.Front].Path = value;
                RaisePropertyChanged(nameof(FrontImagePath));
            }
        }
        /// <summary>
        /// Path to the Back Image.
        /// </summary>
        public string BackImagePath
        {
            get => _ledEntity.ImageInfos[LedEntityView.Back].Path;
            set
            {
                _ledEntity.ImageInfos[LedEntityView.Back].Path = value;
                RaisePropertyChanged(nameof(BackImagePath));
            }
        }

        /// <summary>
        /// All LED's to be displayed (Ellipses).
        /// </summary>
        public List<LedVM> Leds { get; private set; }
        /// <summary>
        /// All LED Groups to be displayed (Rectangles).
        /// </summary>
        public List<LedGroupPropertiesVM> LedGroups { get; }

        public virtual List<Rectangle> FrontLedGroups
        {
            get
            {
                List<Rectangle> res = new List<Rectangle>();
                LedGroups.FindAll(x => x.View == LedEntityView.Front).ForEach(VM => res.Add(VM.Rectangle));
                return res;
            }
        }
        public List<LedVM> FrontLeds
        {
            get => Leds.FindAll(x => x.View == LedEntityView.Front);
        }

        public virtual List<Rectangle> BackLedGroups
        {
            get
            {
                List<Rectangle> res = new List<Rectangle>();
                LedGroups.FindAll(x => x.View == LedEntityView.Back).ForEach(VM => res.Add(VM.Rectangle));
                return res;
            }
        }
        public List<LedVM> BackLeds
        {
            get => Leds.FindAll(X => X.View == LedEntityView.Back);
        }

        /// <summary>
        /// When the EffectService updates led colors to match a specific frame it gets saved here
        /// so on another update call we can check if we maybe jumped a frame or smth else went wrong
        /// </summary>
        public long CurrentFrame;
        private EffectBaseVM _currentEffect;
        public EffectBaseVM CurrentEffect
        {
            get => _currentEffect;
            set
            {
                if (_currentEffect != value)
                {
                    _currentEffect = value;
                    RaisePropertyChanged(nameof(CurrentEffect));
                }
            }
        }

        private ObservableCollection<EffectBaseVM> _effects;
        public ObservableCollection<EffectBaseVM> Effects
        {
            get => _effects;
            set
            {
                if (_effects != value)
                {
                    _effects = value;
                    RaisePropertyChanged(nameof(Effects));
                }
            }
        }

        public virtual Cursor FrontCursor => Cursors.Arrow;
        public virtual Cursor BackCursor => Cursors.Arrow;

        protected Dictionary<LedGroupIdentifier, LedGroupPropertiesVM> _LedIDToGroupVM { get; private set; }
        protected Dictionary<LedGroupPropertiesVM, LedOffset> _LedOffsets { get; private set; }

        public Command SelectLedEntityCommand { get; set; }        

        public Command<MouseEventArgs> FrontImageMouseDownCommand { get; set; }
        public Command<MouseEventArgs> FrontImageMouseMoveCommand { get; set; }

        public Command<MouseEventArgs> BackImageMouseDownCommand { get; set; }
        public Command<MouseEventArgs> BackImageMouseMoveCommand { get; set; }

        public Command<MouseEventArgs> ImageMouseUpCommand { get; set; }

        public LedEntityBaseVM(Model.LedEntity ledEntity)
        {
            LedGroups = new List<LedGroupPropertiesVM>();
            Effects = new ObservableCollection<EffectBaseVM>();
            LedEntity = ledEntity ?? throw new ArgumentNullException();

            _Init();

            //Don't map effects in init function because it will get called every update and is a waste of time
            LedEntity.Effects.ForEach(Effect => Effects.Add(new EffectBaseVM(Effect)));            

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);

            SelectLedEntityCommand = new Command(_OnSelectLedEntityCommand);

            CurrentFrame = 0;

            //If it is a new entity (has no name), give it a default name
            if (Name == null)
                Name = "Default";
        }

        public LedEntityBaseVM(LedEntityCRUDVM ledEntityCRUDVM)
            : this(ledEntityCRUDVM.LedEntity)
        {

        }

        private void _Init()
        {
            LedGroups.Clear();
            _AddExisitingLedGroups();
            _GenerateLedVMs();
            _MapLedGroups();
            _UpdateAllLedPositions();

        }

        public void Update()
        {
            _Init();
            _UpdateAllLedPositions(true);
        }

        /// <summary>
        /// If we edit an existing LedEntity, add's all existing LedGroups to List:LedGroups.
        /// </summary>
        private void _AddExisitingLedGroups()
        {
            foreach (Model.LedBus LedBus in LedEntity.LedBuses.Values)
            {
                foreach (Model.LedGroup LedGroup in LedBus.LedGroups)
                {
                    LedGroups.Add(new LedGroupPropertiesVM(LedGroup));
                }
            }
        }

        /// <summary>
        /// Calculates all Led Positions of all Groups and saves them in List:Leds.
        /// Index all generated LedVMs in List:LedOffsets.
        /// </summary>
        protected void _GenerateLedVMs()
        {
            Leds = new List<LedVM>();

            foreach (LedGroupPropertiesVM ledGroupVM in LedGroups)
            {
                double deltaX = ledGroupVM.SizeOnImage.Width / (ledGroupVM.GridRangeX + 1);
                double deltaY = ledGroupVM.SizeOnImage.Height / (ledGroupVM.GridRangeY + 1);

                int i = 0;
                for (i = 0; i < ledGroupVM.LedGroup.Leds.Count; i++)
                {
                    double x = ledGroupVM.StartPositionOnImage.X + (ledGroupVM.LedGroup.Leds[i].X + ledGroupVM.LedGroup.View.GridLedStartOffset.X + 1) * deltaX;
                    double y = ledGroupVM.StartPositionOnImage.Y + (ledGroupVM.LedGroup.Leds[i].Y + ledGroupVM.LedGroup.View.GridLedStartOffset.Y + 1) * deltaY;

                    Leds.Add(new LedVM(new Point(x, y), ledGroupVM.View));
                }
            }

            _IndexLedVMs();
        }

        /// <summary>
        /// Index all generated LedVMs, to get from a specific SAVED Led to the corresponding DISPLAYED Led.
        /// </summary>
        private void _IndexLedVMs()
        {
            _LedOffsets = new Dictionary<LedGroupPropertiesVM, LedOffset>();

            int offset = 0;
            foreach (LedGroupPropertiesVM ledGroupVM in LedGroups)
            {
                if (_LedOffsets.Count != 0)
                    offset = _LedOffsets.Last().Value.Offset + _LedOffsets.Last().Value.Length;
                if (ledGroupVM.LedGroup.Leds.Count > 0)
                    _LedOffsets.Add(ledGroupVM, new LedOffset(offset, ledGroupVM.LedGroup.Leds.Count, ledGroupVM.View));
                else
                    Console.WriteLine("LedGroup with BusID: {0} and Position: {1} got no Leds. No indexing required.", ledGroupVM.BusID, ledGroupVM.PositionInBus);
            }
        }

        /// <summary>
        /// Updates the position of all groups and leds.
        /// </summary>
        /// <param name="scale">Scale them to main window size or not. Default = false.</param>
        protected void _UpdateAllLedPositions(bool scale = false)
        {
            foreach (LedGroupPropertiesVM LedGroupViewModel in LedGroups)
            {
                _UpdateLedPositions(LedGroupViewModel, scale);
            }
        }

        /// <summary>
        /// Updates the position of a specific group and the leds included.
        /// </summary>
        /// <param name="scale">Scale them to main window size or not. Default = false.</param>
        protected void _UpdateLedPositions(LedGroupPropertiesVM ledGroupViewModel, bool scale = false)
        {
            double deltaX = ledGroupViewModel.SizeOnImage.Width / (ledGroupViewModel.GridRangeX + 1);
            double deltaY = ledGroupViewModel.SizeOnImage.Height / (ledGroupViewModel.GridRangeY + 1);

            for (int i = 0; i < ledGroupViewModel.LedGroup.Leds.Count; i++)
            {
                Point newPosition = new Point
                {
                    X = ledGroupViewModel.StartPositionOnImage.X + (ledGroupViewModel.LedGroup.Leds[i].X + ledGroupViewModel.LedGroup.View.GridLedStartOffset.X + 1) * deltaX,
                    Y = ledGroupViewModel.StartPositionOnImage.Y + (ledGroupViewModel.LedGroup.Leds[i].Y + ledGroupViewModel.LedGroup.View.GridLedStartOffset.Y + 1) * deltaY
                };

                if (scale)
                    Leds[i + _LedOffsets[ledGroupViewModel].Offset].Position = _ScalePoint(newPosition);
                else
                    Leds[i + _LedOffsets[ledGroupViewModel].Offset].Position = newPosition;
            }

            if (scale)
            {
                ledGroupViewModel.StartPositionOnImageScaled = _ScalePoint(ledGroupViewModel.StartPositionOnImage);
                ledGroupViewModel.SizeOnImageScaled = _ScaleSize(ledGroupViewModel.SizeOnImage);
            }
            else
            {
                ledGroupViewModel.StartPositionOnImageScaled = new Point(ledGroupViewModel.StartPositionOnImage.X, ledGroupViewModel.StartPositionOnImage.Y);
                ledGroupViewModel.SizeOnImageScaled = new Size(ledGroupViewModel.SizeOnImage.Width, ledGroupViewModel.SizeOnImage.Height);
            }
        }

        private void _OnSelectLedEntityCommand()
        {
            _SendMessage(MediatorMessages.LedEntitySelectButtonClicked, null);
            _SendMessage(MediatorMessages.TimeLineCollectionChanged, new MediatorMessageData.TimeLineCollectionChangedData(Effects));
        }

        /// <summary>
        /// Maps all exisiting led groups to the corresponding VM.
        /// </summary>
        private void _MapLedGroups()
        {
            _LedIDToGroupVM = new Dictionary<LedGroupIdentifier, LedGroupPropertiesVM>();
            foreach (LedGroupPropertiesVM LedGroupViewModel in LedGroups)
            {                
                _LedIDToGroupVM.Add(new LedGroupIdentifier(LedGroupViewModel.LedGroup.BusID, LedGroupViewModel.LedGroup.PositionInBus), LedGroupViewModel);
            }
        }

        protected void _SetLedColor(List<int> leds, Color color)
        {
            foreach (int ID in leds)
            {
                if (Leds.Count > ID && ID >= 0)
                    Leds[ID].Brush = new SolidColorBrush(color);
            }
        }

        /// <summary>
        /// Sets the color of the specified leds.
        /// </summary>
        /// <param name="ledChangeData"></param>
        public void SetLedColor(Model.LedChangeData ledChangeData)
        {
            foreach (Utility.LedModelID ID in ledChangeData.LedIDs)
            {
                LedGroupIdentifier identifier = new LedGroupIdentifier(ID.BusID, ID.PositionInBus);
                if (_LedIDToGroupVM.ContainsKey(identifier))
                    Leds[ID.Led + _LedOffsets[_LedIDToGroupVM[identifier]].Offset].Brush = new SolidColorBrush(ledChangeData.Color);
                else
                    Console.WriteLine("LedGroup with BusID: {0} and Position: {1} was not indexed.", identifier.BusID, identifier.PositionInBus);
            }
        }

        /// <summary>
        /// Sets the color of the specified leds.
        /// </summary>
        /// <param name="ledChangeData"></param>
        public void SetLedColor(List<Model.LedChangeData> ledChangeData)
        {
            foreach (Model.LedChangeData changeData in ledChangeData)
            {
                SetLedColor(changeData);
            }
        }

        protected Point _ScalePoint(Point point)
        {
            double scaleX = 1.01669877970456;
            double scaleY = 1.01669877970456;
            double offsetX = 0.16666666666668561;
            double offsetY = 15.90170657858522;

            return new Point
            {
                X = point.X * scaleX + offsetX,
                Y = point.Y * scaleY + offsetY
            };
        }

        protected Size _ScaleSize(Size size)
        {
            double scaleX = 1.01669877970456;
            double scaleY = 1.01669877970456;
            double offsetX = 0.16666666666668561;
            double offsetY = 15.90170657858522;

            return new Size
            {
                Width = size.Width * scaleX + offsetX,
                Height = size.Height * scaleY + offsetY
            };
        }

        protected void _SendMessage(MediatorMessages message, object data)
        {
            _Mediator.BroadcastMessage(message, this, data);
        }

        public abstract void RecieveMessage(MediatorMessages message, object sender, object data);
    }

    public class LedOffset
    {
        public int Offset { get; set; }
        public int Length { get; set; }
        public LedEntityView View { get; set; }

        public LedOffset(int offset, int length, LedEntityView view)
        {
            Offset = offset;
            Length = length;
            View = view;
        }
    }
}