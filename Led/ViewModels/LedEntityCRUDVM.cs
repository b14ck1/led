using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Led.ViewModels
{
    class LedEntityCRUDVM : LedEntityBaseVM
    {
        private bool _AddGroup;
        private bool _CreatingGroup;
        private bool _MovingGroup;
        private bool _ResizingGroup;
        
        private Size _MoveDelta;

        private Cursor _frontCursor;
        public new Cursor FrontCursor
        {
            get => _frontCursor;
            set
            {
                if (_frontCursor != value)
                {
                    _frontCursor = value;
                    RaisePropertyChanged(nameof(FrontCursor));
                }
            }
        }

        private Cursor _backCursor;
        public new Cursor BackCursor
        {
            get => _backCursor;
            set
            {
                if (_backCursor != value)
                {
                    _backCursor = value;
                    RaisePropertyChanged(nameof(BackCursor));
                }
            }
        }

        public LedGroupPropertiesVM CurrentLedGroup { get; set; }

        public Command NewFrontImageCommand { get; set; }
        public Command NewBackImageCommand { get; set; }

        public Command AddLedGroupCommand { get; set; }
        public Command EditLedGroupCommand { get; set; }
        public Command DeleteLedGroupCommand { get; set; }

        public Command CloseWindowCommand { get; set; }

        public LedEntityCRUDVM(Model.LedEntity ledEntity)
            : base(ledEntity)
        {
            NewFrontImageCommand = new Command(_OnNewFrontImmage);
            NewBackImageCommand = new Command(_OnNewBackImmage);

            AddLedGroupCommand = new Command(_OnAddLedGroupCommand, () => !(_AddGroup || (FrontImagePath == null && BackImagePath == null)));
            EditLedGroupCommand = new Command(_OnEditLedGroupCommand, () => CurrentLedGroup != null);
            DeleteLedGroupCommand = new Command(_OnDeleteLedGroupCommand, () => CurrentLedGroup != null);

            FrontImageMouseDownCommand = new Command<MouseEventArgs>(_OnFrontImageMouseDownCommand);
            FrontImageMouseMoveCommand = new Command<MouseEventArgs>(_OnFrontImageMouseMoveCommand);

            BackImageMouseDownCommand = new Command<MouseEventArgs>(_OnBackImageMouseDownCommand);
            BackImageMouseMoveCommand = new Command<MouseEventArgs>(_OnBackImageMouseMoveCommand);

            ImageMouseUpCommand = new Command<MouseEventArgs>(_OnImageMouseUpCommand);

            CloseWindowCommand = new Command(_OnCloseWindowCommand, _CanExecuteClosing);
        }

        private void _OnNewFrontImmage()
        {
            string Path = App.Instance.IOService.OpenFileDialog();
            if (Path != "")
            {
                FrontImagePath = Path;
                BitmapImage Image = new BitmapImage(new Uri(FrontImagePath));
                LedEntity.ImageInfos[LedEntityView.Front].Size.Width = Image.PixelWidth;
                LedEntity.ImageInfos[LedEntityView.Front].Size.Height = Image.PixelHeight;
            }

            AddLedGroupCommand.RaiseCanExecuteChanged();
        }
        private void _OnNewBackImmage()
        {
            string Path = App.Instance.IOService.OpenFileDialog();
            if (Path != "")
            { 
                BackImagePath = Path;
                BitmapImage Image = new BitmapImage(new Uri(BackImagePath));
                LedEntity.ImageInfos[LedEntityView.Back].Size.Width = Image.PixelWidth;
                LedEntity.ImageInfos[LedEntityView.Back].Size.Height = Image.PixelHeight;
            }

            AddLedGroupCommand.RaiseCanExecuteChanged();
        }

        private void _OnAddLedGroupCommand()
        {
            _AddGroup = true;
            AddLedGroupCommand.RaiseCanExecuteChanged();
        }
        private void _OnEditLedGroupCommand()
        {
            App.Instance.WindowService.ShowNewWindow(new Views.CRUDs.LedGroupCRUD(), CurrentLedGroup);
            _GenerateLedVMs();
            RaisePropertyChanged("FrontLeds");
            RaisePropertyChanged("BackLeds");
        }
        private void _OnDeleteLedGroupCommand()
        {
            LedGroups.Remove(CurrentLedGroup);
            CurrentLedGroup = null;

            _GenerateLedVMs();
            RaisePropertyChanged("FrontLeds");
            RaisePropertyChanged("BackLeds");
            RaisePropertyChanged("BackLedGroups");
            RaisePropertyChanged("FrontLedGroups");

            EditLedGroupCommand.RaiseCanExecuteChanged();
            DeleteLedGroupCommand.RaiseCanExecuteChanged();
        }

        private void _OnFrontImageMouseDownCommand(MouseEventArgs e)
        {
            if (_AddGroup)
            {
                _AddLedGroup(e, LedEntityView.Front);
                RaisePropertyChanged(nameof(FrontLedGroups));
            }
            else if (!_ScanForLedGroups(e, LedGroups.FindAll(x => x.View == LedEntityView.Front)))
            {
                CurrentLedGroup = null;
                RaisePropertyChanged(nameof(CurrentLedGroup));
                EditLedGroupCommand.RaiseCanExecuteChanged();
                DeleteLedGroupCommand.RaiseCanExecuteChanged();
            }
        }
        private void _OnBackImageMouseDownCommand(MouseEventArgs e)
        {
            if (_AddGroup)
            {
                _AddLedGroup(e, LedEntityView.Back);
                RaisePropertyChanged(nameof(BackLedGroups));
            }
            else if (!_ScanForLedGroups(e, LedGroups.FindAll(x => x.View == LedEntityView.Back)))
            {
                CurrentLedGroup = null;
                RaisePropertyChanged(nameof(CurrentLedGroup));
                EditLedGroupCommand.RaiseCanExecuteChanged();
                DeleteLedGroupCommand.RaiseCanExecuteChanged();
            }
        }

        private void _OnFrontImageMouseMoveCommand(MouseEventArgs e)
        {
            if (_CreatingGroup || _ResizingGroup)
            {
                _ResizeGroup(e);
                return;
            }
            else if (_MovingGroup)
            {
                _MoveGroup(e);
                return;
            }

            //Bisschen Bling Bling
            FrontCursor = _ChangeCursor(e, LedGroups.FindAll(x => x.View == LedEntityView.Front));
        }
        private void _OnBackImageMouseMoveCommand(MouseEventArgs e)
        {
            if (_CreatingGroup || _ResizingGroup)
            {
                _ResizeGroup(e);
                return;
            }
            else if (_MovingGroup)
            {
                _MoveGroup(e);
                return;
            }

            //Bisschen Bling Bling
            BackCursor = _ChangeCursor(e, LedGroups.FindAll(x => x.View == LedEntityView.Back));
        }

        private void _OnImageMouseUpCommand(MouseEventArgs e)
        {
            _ResetFlags();
        }

        private bool _CanExecuteClosing()
        {
            //Returns true if there are no groups left which need a correction
            if (_CheckBusDefinitions().NeedCorrection.Values.Any(x => x))
            {
                MessageBox.Show("They are some groups which bus definitions are overlapping.");
                return false;
            }
            return true;
        }

        private void _OnCloseWindowCommand()
        {
            LedEntity.LedBuses = new Dictionary<byte, Model.LedBus>();

            foreach (LedGroupPropertiesVM LedGroupViewModel in LedGroups)
            {
                if (!LedEntity.LedBuses.ContainsKey(LedGroupViewModel.LedGroup.BusID))
                    LedEntity.LedBuses.Add(LedGroupViewModel.LedGroup.BusID, new Model.LedBus());

                LedEntity.LedBuses[LedGroupViewModel.LedGroup.BusID].LedGroups.Add(LedGroupViewModel.LedGroup);
            }

            foreach(Model.LedBus LedBus in LedEntity.LedBuses.Values)
            {
                LedBus.LedGroups.Sort((Model.LedGroup x, Model.LedGroup y) => x.PositionInBus > y.PositionInBus ? 1 : -1);
            }

            App.Instance.WindowService.CloseWindow(this);
        }

        private void _AddLedGroup(MouseEventArgs e, LedEntityView view)
        {
            LedGroups.Add(new LedGroupPropertiesVM()
            {
                StartPositionOnImage = e.GetPosition((IInputElement)e.Source),
                SizeOnImage = new Size(0, 0),
                View = view
            });
            CurrentLedGroup = LedGroups.Last();
            _SendMessage(MediatorMessages.GroupBusDefinitionsNeedCorrectionChanged, _CheckBusDefinitions());
            _CreatingGroup = true;            
        }

        private bool _ScanForLedGroups(MouseEventArgs e, List<LedGroupPropertiesVM> ledGroups)
        {
            Point MousePosition = e.GetPosition((IInputElement)e.Source);
            foreach (var LedGroup in ledGroups)
            {
                Point Start = LedGroup.StartPositionOnImage;
                Point End = new Point(Start.X + LedGroup.SizeOnImage.Width, Start.Y + LedGroup.SizeOnImage.Height);

                if (MousePosition.X <= End.X + 5 && MousePosition.X >= End.X - 5 && MousePosition.Y <= End.Y + 5 && MousePosition.Y >= End.Y - 5)
                {
                    CurrentLedGroup = LedGroup;
                    RaisePropertyChanged(nameof(CurrentLedGroup));
                    EditLedGroupCommand.RaiseCanExecuteChanged();
                    DeleteLedGroupCommand.RaiseCanExecuteChanged();
                    _ResizingGroup = true;
                    return true;
                }

                if (MousePosition.X <= End.X && MousePosition.X >= Start.X && MousePosition.Y <= End.Y && MousePosition.Y >= Start.Y)
                {
                    CurrentLedGroup = LedGroup;
                    RaisePropertyChanged(nameof(CurrentLedGroup));
                    EditLedGroupCommand.RaiseCanExecuteChanged();
                    DeleteLedGroupCommand.RaiseCanExecuteChanged();
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        _MoveDelta = new Size(MousePosition.X - Start.X, MousePosition.Y - Start.Y);
                        _MovingGroup = true;
                    }
                    return true;
                }
            }

            return false;
        }

        private void _ResizeGroup(MouseEventArgs e)
        {
            Point Start = CurrentLedGroup.StartPositionOnImage;
            Point MousePosition = e.GetPosition((IInputElement) e.Source);
            double deltaX = MousePosition.X - Start.X;
            double deltaY = MousePosition.Y - Start.Y;

            if (deltaX< 5)
                deltaX = 5;
            if (deltaY< 5)
                deltaY = 5;                    

            CurrentLedGroup.SizeOnImage = new Size(deltaX, deltaY);
            _UpdateLedPositions(CurrentLedGroup);
        }

        private void _MoveGroup(MouseEventArgs e)
        {
            Point MousePosition = e.GetPosition((IInputElement)e.Source);
            double PositionX = MousePosition.X - _MoveDelta.Width;
            double PositionY = MousePosition.Y - _MoveDelta.Height;

            if (PositionX < 0)
                PositionX = 0;
            if (PositionY < 0)
                PositionY = 0;

            CurrentLedGroup.StartPositionOnImage = new Point(PositionX, PositionY);
            _UpdateLedPositions(CurrentLedGroup);
        }

        private Cursor _ChangeCursor(MouseEventArgs e, List<LedGroupPropertiesVM> ledGroups)
        {
            foreach (var LedGroup in ledGroups)
            {
                Point Start = LedGroup.StartPositionOnImage;
                Point MousePosition = e.GetPosition((IInputElement)e.Source);
                Point End = new Point(Start.X + LedGroup.SizeOnImage.Width, Start.Y + LedGroup.SizeOnImage.Height);

                if (MousePosition.X <= End.X + 5 && MousePosition.X >= End.X - 5 && MousePosition.Y <= End.Y + 5 && MousePosition.Y >= End.Y - 5)
                {
                    return Cursors.SizeNWSE;
                }
                else if (MousePosition.X <= End.X && MousePosition.X >= Start.X && MousePosition.Y <= End.Y && MousePosition.Y >= Start.Y)
                {
                    return Cursors.SizeAll;
                }
            }

            return Cursors.Arrow;
        }

        private void _ResetFlags()
        {
            _AddGroup = false;
            _CreatingGroup = false;
            _MovingGroup = false;
            _ResizingGroup = false;
            AddLedGroupCommand.RaiseCanExecuteChanged();
        }

        public override void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.LedEntitySelectButtonClicked:
                    break;
                case MediatorMessages.EffectVMEditSelectedLedsClicked:
                    break;
                case MediatorMessages.GroupBusDefinitionsChanged:
                    _SendMessage(MediatorMessages.GroupBusDefinitionsNeedCorrectionChanged, _CheckBusDefinitions());
                    break;
                case MediatorMessages.GroupBusDefinitionsNeedCorrectionChanged:
                    break;
                default:
                    break;
            }
        }

        private MediatorMessageData.GroupBusDefinitionsNeedCorrectionChanged _CheckBusDefinitions()
        {
            List<LedGroupPropertiesVM> temp = new List<LedGroupPropertiesVM>();
            foreach (var Group in LedGroups)
            {
                if ((LedGroups.FindAll(x => x.BusID == Group.BusID && x.PositionInBus == Group.PositionInBus).Count > 1))
                {
                    temp.AddRange(LedGroups.FindAll(x => x.BusID == Group.BusID && x.PositionInBus == Group.PositionInBus));
                }
            }
            temp = temp.Distinct().ToList();

            Dictionary<LedGroupPropertiesVM, bool> res = new Dictionary<LedGroupPropertiesVM, bool>();
            foreach (var Group in LedGroups)
            {
                if (temp.Contains(Group))
                    res.Add(Group, true);
                else
                    res.Add(Group, false);
            }
            return new MediatorMessageData.GroupBusDefinitionsNeedCorrectionChanged(res);            
        }
    }
}