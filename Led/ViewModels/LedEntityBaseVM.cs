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
    abstract class LedEntityBaseVM : INPC, Interfaces.IParticipant
    {
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

        private Services.MediatorService _Mediator;

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

        public virtual Cursor FrontCursor => Cursors.Arrow;
        public virtual Cursor BackCursor => Cursors.Arrow;

        protected Dictionary<LedGroupIdentifier, LedGroupPropertiesVM> LedIDToGroupVM { get; private set; }
        protected Dictionary<LedGroupPropertiesVM, LedOffset> LedOffsets { get; private set; }

        public Command SelectLedEntityCommand { get; set; }
        public static string SelectLedEntityMessage = "LedEntitySelected";

        public Command<MouseEventArgs> FrontImageMouseDownCommand { get; set; }
        public Command<MouseEventArgs> FrontImageMouseMoveCommand { get; set; }

        public Command<MouseEventArgs> BackImageMouseDownCommand { get; set; }
        public Command<MouseEventArgs> BackImageMouseMoveCommand { get; set; }

        public Command<MouseEventArgs> ImageMouseUpCommand { get; set; }

        public LedEntityBaseVM(Model.LedEntity ledEntity)
        {
            LedEntity = ledEntity ?? throw new ArgumentNullException();

            //Initialize all that shit
            LedGroups = new List<LedGroupPropertiesVM>();
            AddExisitingLedGroups();
            GenerateLedVMs();
            MapLedGroups();
            UpdateAllLedPositions();

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);

            SelectLedEntityCommand = new Command(OnSelectLedEntityCommand);

            //If it is a new entity (has no name), give it a default name
            if (Name == null)
                Name = "Default";
        }

        public LedEntityBaseVM(LedEntityCRUDVM ledEntityCRUDVM)
            :this(ledEntityCRUDVM.LedEntity)
        {

        }

        /// <summary>
        /// If we edit an existing LedEntity, add's all existing LedGroups to List:LedGroups.
        /// </summary>
        private void AddExisitingLedGroups()
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
        protected void GenerateLedVMs()
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

            IndexLedVMs();
        }

        /// <summary>
        /// Index all generated LedVMs, to get from a specific SAVED Led to the corresponding DISPLAYED Led.
        /// </summary>
        private void IndexLedVMs()
        {
            LedOffsets = new Dictionary<LedGroupPropertiesVM, LedOffset>();

            int offset = 0;
            foreach (LedGroupPropertiesVM ledGroupVM in LedGroups)
            {
                if (LedOffsets.Count != 0)
                    offset = LedOffsets.Last().Value.Offset + LedOffsets.Last().Value.Length;
                LedOffsets.Add(ledGroupVM, new LedOffset(offset, ledGroupVM.LedGroup.Leds.Count - 1, ledGroupVM.View));
            }
        }
        
        /// <summary>
        /// Updates the position of all groups and leds.
        /// </summary>
        /// <param name="scale">Scale them to main window size or not. Default = false.</param>
        protected void UpdateAllLedPositions(bool scale = false)
        {
            foreach (LedGroupPropertiesVM LedGroupViewModel in LedGroups)
            {
                UpdateLedPositions(LedGroupViewModel, scale);
            }
        }

        /// <summary>
        /// Updates the position of a specific group and the leds included.
        /// </summary>
        /// <param name="scale">Scale them to main window size or not. Default = false.</param>
        protected void UpdateLedPositions(LedGroupPropertiesVM ledGroupViewModel, bool scale = false)
        {
            double scaleX = 1;
            double scaleY = 1;
            double offsetX = 0;
            double offsetY = 0;

            if (scale)
            {
                scaleX = 1.01669877970456;
                scaleY = 1.01669877970456;
                offsetX = 0.16666666666668561;
                offsetY = 15.90170657858522;
            }

            double deltaX = ledGroupViewModel.SizeOnImage.Width / (ledGroupViewModel.GridRangeX + 1);
            double deltaY = ledGroupViewModel.SizeOnImage.Height / (ledGroupViewModel.GridRangeY + 1);

            if (offsetY < 0)
                offsetY = 0;

            for (int i = 0; i < ledGroupViewModel.LedGroup.Leds.Count; i++)
            {
                double x = ledGroupViewModel.StartPositionOnImage.X + (ledGroupViewModel.LedGroup.Leds[i].X + ledGroupViewModel.LedGroup.View.GridLedStartOffset.X + 1) * deltaX;
                double y = ledGroupViewModel.StartPositionOnImage.Y + (ledGroupViewModel.LedGroup.Leds[i].Y + ledGroupViewModel.LedGroup.View.GridLedStartOffset.Y + 1) * deltaY;

                Leds[i + LedOffsets[ledGroupViewModel].Offset].Position = new Point(x * scaleX + offsetX, y * scaleY + offsetY);
            }
            
            ledGroupViewModel.StartPositionOnImageScaled = new Point(ledGroupViewModel.StartPositionOnImage.X * scaleX + offsetX, ledGroupViewModel.StartPositionOnImage.Y * scaleY + offsetY);
            ledGroupViewModel.SizeOnImageScaled = new Size(ledGroupViewModel.SizeOnImage.Width * scaleX, ledGroupViewModel.SizeOnImage.Height * scaleY);
        }

        private void OnSelectLedEntityCommand()
        {
            SendMessage(MediatorMessages.LedEntitySelected, null);
        }

        /// <summary>
        /// Maps all exisiting led groups to the corresponding VM.
        /// </summary>
        private void MapLedGroups()
        {
            LedIDToGroupVM = new Dictionary<LedGroupIdentifier, LedGroupPropertiesVM>();
            foreach (LedGroupPropertiesVM LedGroupViewModel in LedGroups)
            {
                //NEED: Every added group needs to have a different identifier (BusID, PositionInBus)
                //LedIDToGroupVM.Add(new LedGroupIdentifier(LedGroupViewModel.LedGroup.BusID, LedGroupViewModel.LedGroup.PositionInBus), LedGroupViewModel);
            }
        }

        /// <summary>
        /// Sets the color of the specified leds.
        /// </summary>
        /// <param name="LedChangeData"></param>
        public void SetLedColor(Model.LedChangeData LedChangeData)
        {
            foreach (Utility.LedModelID ID in LedChangeData.LedIDs)
            {
                Leds[ID.Led + LedOffsets[LedIDToGroupVM[new LedGroupIdentifier(ID.BusID, ID.PositionInBus)]].Offset].Brush = new SolidColorBrush(LedChangeData.Color);
            }
        }

        /// <summary>
        /// Sets the color of the specified leds.
        /// </summary>
        /// <param name="LedChangeData"></param>
        public void SetLedColor(List<Model.LedChangeData> LedChangeData)
        {
            foreach (Model.LedChangeData ChangeData in LedChangeData)
            {
                SetLedColor(ChangeData);
            }
        }

        protected void SendMessage(MediatorMessages message, object data)
        {
            _Mediator.BroadcastMessage(message, this, data);
        }

        public virtual void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            //throw new NotImplementedException();
        }
    }

    class LedOffset
    {
        public int Offset;
        public int Length;
        public LedEntityView View;

        public LedOffset(int Offset, int Length, LedEntityView View)
        {
            this.Offset = Offset;
            this.Length = Length;
            this.View = View;
        }       
    }
}