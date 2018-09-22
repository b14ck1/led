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
    class MainWindowVM : INPC, IParticipant
    {
        private Services.MediatorService _Mediator;

        private Model.Project _project;
        public Model.Project Project
        {
            get => _project;
            set
            {
                LedEntities.Clear();
                _CurrentLedEntity = null;
                _LedEntityView.DataContext = null;
                _LedEntityView.DataContext = _CurrentLedEntity;
                EditLedEntityCommand.RaiseCanExecuteChanged();

                _project = value;
                                
                Project.LedEntities.ForEach(LedEntity => LedEntities.Add(new LedEntitySelectVM(LedEntity)));

                RaiseAllPropertyChanged();
                SaveProjectCommand.RaiseCanExecuteChanged();
                NewLedEntityCommand.RaiseCanExecuteChanged();
                AddAudioCommand.RaiseCanExecuteChanged();
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

        private LedEntityBaseVM _CurrentLedEntity;        
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

        private EffectBaseVM _CurrentEffect => _CurrentLedEntity.CurrentEffect;
        private Views.Controls.MainWindow.EffectProperties _EffectView;

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
                    RaisePropertyChanged();
                }
            }
        }

        public Command SaveProjectCommand { get; set; }
        public Command LoadProjectCommand { get; set; }

        public Command NewProjectCommand { get; set; }
        public Command NewLedEntityCommand { get; set; }
        public Command AddAudioCommand { get; set; }

        public Command EditLedEntityCommand { get; set; }

        public MainWindowVM(Views.MainWindow mainWindow, Views.Controls.LedEntityOverview ledEntity, Views.Controls.MainWindow.EffectProperties effectView, Views.Controls.MainWindow.AudioUserControl audioUserControl)
        {
            _MainWindow = mainWindow;
            _LedEntityView = ledEntity;
            _LedEntityView.DataContext = _CurrentLedEntity;
            
            _EffectView = effectView;

            LedEntities = new ObservableCollection<LedEntityBaseVM>();

            //_CurrentEffect = new EffectBaseVM();
            //effectView.DataContext = _CurrentEffect;
            _AudioUserControl = audioUserControl;
            audioUserControl.DataContext = AudioUserControlVM;

            SaveProjectCommand = new Command(_OnSaveProjectCommand, () => Project != null);
            LoadProjectCommand = new Command(_OnLoadProjectCommand);

            NewProjectCommand = new Command(_OnNewProjectCommand);
            NewLedEntityCommand = new Command(_OnNewLedEntityCommand, () => Project != null);
            EditLedEntityCommand = new Command(_OnEditLedEntityCommand, () => _CurrentLedEntity != null);
            AddAudioCommand = new Command(_OnAddAudioCommand, () => Project != null);

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        private void _OnSaveProjectCommand()
        {
            System.IO.File.WriteAllText(@"C:\Users\Robin\Desktop\test.json", Newtonsoft.Json.JsonConvert.SerializeObject(Project, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            }));
        }

        private void _OnLoadProjectCommand()
        {
            Project = JsonConvert.DeserializeObject<Model.Project>(System.IO.File.ReadAllText(@"C:\Users\Robin\Desktop\test.json"), new JsonSerializerSettings
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
                Project = new Model.Project(_newProjectDialogVM.ProjectName);
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

            App.Instance.WindowService.ShowNewWindow(ledEntityCRUDView, new LedEntityCRUDVM(_CurrentLedEntity.LedEntity));
            
            _CurrentLedEntity.Update();            
            _LedEntityView.DataContext = null;
            _LedEntityView.DataContext = _CurrentLedEntity;
        }

        //private void OnSelectedLedEntity(object sender, EventArgs e)
        //{
        //    //LedEntitySelectViewModel test = new LedEntitySelectViewModel(((LedEntityViewModel)sender).LedEntity);
        //    //_ledEntityView.DataContext = test;
        //    _currentLedEntity = (LedEntityBaseVM)sender;
        //    _ledEntityView.DataContext = (LedEntityBaseVM)sender;         
        //}

        private void _OnAddAudioCommand()
        {
            var fileFilter = "*.mp3;*.m4a;*.wav;*.flac";
            String audioFileName = App.Instance.IOService.OpenFileDialog($"Audio-Dateien ({fileFilter})|{fileFilter}");

            AudioUserControlVM = new AudioUserControlVM(audioFileName);
            _AudioUserControl.DataContext = AudioUserControlVM;
        }

        public virtual void RecieveMessage(MediatorMessages message, object sender, object data)
        {
            switch (message)
            {
                case MediatorMessages.LedEntitySelectButtonClicked:
                    _CurrentLedEntity = (sender as LedEntityBaseVM);                    
                    _LedEntityView.DataContext = _CurrentLedEntity;
                    _EffectView.DataContext = _CurrentEffect;
                    EditLedEntityCommand.RaiseCanExecuteChanged();
                    break;
                default:
                    break;
            }    
        }
    }
}