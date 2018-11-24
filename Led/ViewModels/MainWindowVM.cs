using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Led.Interfaces;
using Newtonsoft.Json;

namespace Led.ViewModels
{
    public class MainWindowVM : INPC, IParticipant
    {
        private Services.MediatorService _Mediator;

        private Model.Project _project;
        public Model.Project Project
        {
            get => _project;
            set
            {
                _project = value;
                _InitProject();
            }
        }

        private Views.Controls.LedEntityOverview _LedEntityView;
        private Views.MainWindow _MainWindow;

        public string ProjectName
        {
            get => Project == null ? "Sickes LED Progr jaaa" : Project.ProjectName;
            set
            {
                if (Project.ProjectName != value)
                {
                    Project.ProjectName = value;
                    RaisePropertyChanged(nameof(ProjectName));
                }
            }
        }

        public int MainWindowWidth
        {
            get => Defines.MainWindowWidth;
        }
        public int MainWindowHeight
        {
            get => Defines.MainWindowHeight;
        }

        public LedEntityBaseVM CurrentLedEntity;        
        private ObservableCollection<LedEntityBaseVM> _ledEntities;
        public ObservableCollection<LedEntityBaseVM> LedEntities
        {
            get => _ledEntities;
            set
            {
                if (_ledEntities != value)
                {
                    _ledEntities = value;
                    RaisePropertyChanged(nameof(LedEntities));
                }
            }
        }

        private EffectBaseVM _CurrentEffect => CurrentLedEntity.CurrentEffect;
        private Views.Controls.MainWindow.EffectProperties _EffectView;

        private Views.Controls.MainWindow.TimelineUserControl _TimelineUserControl;
        private Views.Controls.MainWindow.AudioUserControl _AudioUserControl;
        private AudioUserControlVM _audioUserControlVM;
        public AudioUserControlVM AudioUserControlVM
        {
            get => _audioUserControlVM;
            set
            {
                if (_audioUserControlVM != value)
                {
                    _audioUserControlVM = value;

                    _SendMessage(MediatorMessages.AudioProperty_NewAudio, new MediatorMessageData.AudioProperty_NewAudio(Project.AudioProperty));

                    RaisePropertyChanged();
                }
            }
        }

        private Views.Controls.MainWindow.NetworkClientOverview _NetworkClientOverview;
        private NetworkClientOverviewVM _NetworkClientOverviewVM;

        public Command SaveProjectCommand { get; set; }
        public Command LoadProjectCommand { get; set; }

        public Command NewProjectCommand { get; set; }
        public Command NewLedEntityCommand { get; set; }

        public Command AddAudioCommand { get; set; }
        public Command SendConfigCommand { get; set; }
        public Command SendEffectCommand { get; set; }

        public Command EditLedEntityCommand { get; set; }
        public Command AddEffectCommand { get; set; }

        public MainWindowVM(Views.MainWindow mainWindow, Views.Controls.LedEntityOverview ledEntity,
            Views.Controls.MainWindow.EffectProperties effectView,
            Views.Controls.MainWindow.TimelineUserControl timelineUserControl,
            Views.Controls.MainWindow.AudioUserControl audioUserControl,
            Views.Controls.MainWindow.NetworkClientOverview networkClientOverview)
        {
            //Init
            LedEntities = new ObservableCollection<LedEntityBaseVM>();
            _NetworkClientOverviewVM = new NetworkClientOverviewVM();

            //Get Refs
            _MainWindow = mainWindow;
            _LedEntityView = ledEntity;
            _EffectView = effectView;
            _TimelineUserControl = timelineUserControl;
            _AudioUserControl = audioUserControl;
            _NetworkClientOverview = networkClientOverview;

            //Set DataContexts
            _LedEntityView.DataContext = CurrentLedEntity;
            _AudioUserControl.DataContext = AudioUserControlVM;
            _NetworkClientOverview.DataContext = _NetworkClientOverviewVM;

            //Init Commands
            SaveProjectCommand = new Command(_OnSaveProjectCommand, () => Project != null);
            LoadProjectCommand = new Command(_OnLoadProjectCommand);

            NewProjectCommand = new Command(_OnNewProjectCommand);
            NewLedEntityCommand = new Command(_OnNewLedEntityCommand, () => Project != null);
            EditLedEntityCommand = new Command(_OnEditLedEntityCommand, () => CurrentLedEntity != null);
            AddEffectCommand = new Command(_OnAddEffectCommand, () => CurrentLedEntity != null && Project.AudioProperty != null);
            AddAudioCommand = new Command(_OnAddAudioCommand, () => Project != null);
            SendConfigCommand = new Command(_OnSendConfigCommand, () => Project != null);
            SendEffectCommand = new Command(_OnSendEffectCommand, () => Project != null);

            //Init Mediator
            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);            
        }

        private void _OnSaveProjectCommand()
        {
            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop\Led\test.json", Newtonsoft.Json.JsonConvert.SerializeObject(Project, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            }));
        }

        private void _OnLoadProjectCommand()
        {
            Project = JsonConvert.DeserializeObject<Model.Project>(System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Desktop\Led\test.json"), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        private void _OnNewProjectCommand()
        {
            View.SingleTextDialog _newProjectDialog = new View.SingleTextDialog();
            NewProjectDialogVM _newProjectDialogVM = new NewProjectDialogVM();

            App.Instance.WindowService.ShowNewWindow(_newProjectDialog, _newProjectDialogVM);

            if (_newProjectDialogVM.DialogResult)
                Project = new Model.Project(_newProjectDialogVM.Text);
        }

        private void _OnNewLedEntityCommand()
        {
            Project.LedEntities.Add(new Model.LedEntity());

            Views.CRUDs.LedEntityCRUD ledEntityCRUDView = new Views.CRUDs.LedEntityCRUD();
            LedEntityCRUDVM ledEntityEditVM = new LedEntityCRUDVM(Project.LedEntities.Last());

            Views.Controls.CRUDs.LedEntityGroupProperties _ledGroupProperties = new Views.Controls.CRUDs.LedEntityGroupProperties();
            ledEntityCRUDView.InnerGrid.Children.Add(_ledGroupProperties);

            Views.Controls.LedEntityOverview _ledEntity = new Views.Controls.LedEntityOverview();
            Grid.SetColumn(_ledEntity, 1);
            ledEntityCRUDView.Grid.Children.Add(_ledEntity);

            App.Instance.WindowService.ShowNewWindow(ledEntityCRUDView, ledEntityEditVM);

            LedEntities.Add(new LedEntitySelectVM(ledEntityEditVM.LedEntity));
        }

        private void _OnEditLedEntityCommand()
        {            
            Views.CRUDs.LedEntityCRUD ledEntityCRUDView = new Views.CRUDs.LedEntityCRUD();
            Views.Controls.CRUDs.LedEntityGroupProperties _ledGroupProperties = new Views.Controls.CRUDs.LedEntityGroupProperties();
            ledEntityCRUDView.InnerGrid.Children.Add(_ledGroupProperties);

            Views.Controls.LedEntityOverview ledEntity = new Views.Controls.LedEntityOverview();
            Grid.SetColumn(ledEntity, 1);
            ledEntityCRUDView.Grid.Children.Add(ledEntity);

            App.Instance.WindowService.ShowNewWindow(ledEntityCRUDView, new LedEntityCRUDVM(CurrentLedEntity.LedEntity));

            CurrentLedEntity.Update();
            _LedEntityView.DataContext = null;
            _LedEntityView.DataContext = CurrentLedEntity;
        }

        private void _OnAddEffectCommand()
        {
            (CurrentLedEntity as LedEntitySelectVM).AddEffect();            
        }

        private void _OnAddAudioCommand()
        {
            var fileFilter = "*.mp3;*.m4a;*.wav;*.flac";
            string filePath = App.Instance.IOService.OpenFileDialog($"Audio-Dateien ({fileFilter})|{fileFilter}");
            if (!string.IsNullOrEmpty(filePath))
            {                    
                Project.AudioProperty = new Model.AudioProperty(filePath);
                _InitAudioUserControl();
            }
            AddEffectCommand.RaiseCanExecuteChanged();
        }

        private void _OnSendConfigCommand()
        {
            foreach (var x in LedEntities)
            {
                App.Instance.ConnectivityService.SendMessage(TcpMessages.Config, x.ClientID);
            }            
        }

        private void _OnSendEffectCommand()
        {
            App.Instance.EffectService.RenderAllEntities();
            foreach (var x in LedEntities)
            {
                App.Instance.ConnectivityService.SendMessage(TcpMessages.Effects, x.ClientID);
            }
        }

        private void _InitProject()
        {
            //Clear all old values
            LedEntities.Clear();
            CurrentLedEntity = null;
            _LedEntityView.DataContext = null;
            _LedEntityView.DataContext = CurrentLedEntity;

            //Add existing shit if there is something
            Project.LedEntities.ForEach(LedEntity => LedEntities.Add(new LedEntitySelectVM(LedEntity)));

            if (!string.IsNullOrEmpty(Project.AudioProperty?.FilePath))
                _InitAudioUserControl();

            _NetworkClientOverviewVM.RemapClients();

            //Update the View and all Commands
            RaiseAllPropertyChanged();
            EditLedEntityCommand.RaiseCanExecuteChanged();
            AddEffectCommand.RaiseCanExecuteChanged();
            SaveProjectCommand.RaiseCanExecuteChanged();
            NewLedEntityCommand.RaiseCanExecuteChanged();
            AddAudioCommand.RaiseCanExecuteChanged();
            SendConfigCommand.RaiseCanExecuteChanged();
            SendEffectCommand.RaiseCanExecuteChanged();
        }

        private void _InitAudioUserControl()
        {
            if (AudioUserControlVM != null)
                AudioUserControlVM.Dispose();
            AudioUserControlVM = new AudioUserControlVM(Project.AudioProperty.FilePath);
            _AudioUserControl.DataContext = AudioUserControlVM;            
        }

        protected void _SendMessage(MediatorMessages message, object data)
        {
            _Mediator.BroadcastMessage(message, this, data);
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.LedEntity_SelectButtonClicked:
                    CurrentLedEntity = (sender as LedEntityBaseVM);
                    _LedEntityView.DataContext = CurrentLedEntity;
                    _EffectView.DataContext = _CurrentEffect;
                    EditLedEntityCommand.RaiseCanExecuteChanged();
                    AddEffectCommand.RaiseCanExecuteChanged();
                    SendConfigCommand.RaiseCanExecuteChanged();
                    break;
                case MediatorMessages.TimeLineEffectSelected:
                    CurrentLedEntity.CurrentEffect = (data as MediatorMessageData.TimeLineEffectSelectedData).EffectBaseVM;                    
                    break;
                case MediatorMessages.LedEntitySelectVM_CurrentEffectChanged:
                    _EffectView.DataContext = _CurrentEffect;
                    break;
                default:
                    break;
            }
        }
    }
}