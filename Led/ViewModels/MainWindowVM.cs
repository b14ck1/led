using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Led.Interfaces;

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
                _project = value;
                RaiseAllPropertyChanged();
                NewLedEntityCommand.RaiseCanExecuteChanged();
                AddAudioCommand.RaiseCanExecuteChanged();
            }
        }

        private Views.Controls.LedEntityOverview _LedEntityView;
        private Views.MainWindow _MainWindow;

        public string ProjectName
        {
            get => _project == null ? "Sickes LED Progr jaaa" : _project.ProjectName;
            set
            {
                if (_project.ProjectName != value)
                {
                    _project.ProjectName = value;
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
                    RaisePropertyChanged("LedEntities");
                }
            }
        }

        private Views.Controls.MainWindow.EffectProperties _EffectView;
        private EffectBaseVM _EffectViewModel;

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

        public Command NewProjectCommand { get; set; }
        public Command NewLedEntityCommand { get; set; }
        public Command AddAudioCommand { get; set; }

        public MainWindowVM(Views.MainWindow mainWindow, Views.Controls.LedEntityOverview ledEntity, Views.Controls.MainWindow.EffectProperties effectView, Views.Controls.MainWindow.AudioUserControl audioUserControl)
        {
            _MainWindow = mainWindow;
            _LedEntityView = ledEntity;
            ledEntity.DataContext = null;
            _EffectView = effectView;

            _ledEntities = new ObservableCollection<LedEntityBaseVM>();
            
            _EffectViewModel = new EffectBaseVM();            
            effectView.DataContext = _EffectViewModel;
            _AudioUserControl = audioUserControl;
            audioUserControl.DataContext = AudioUserControlVM;

            NewProjectCommand = new Command(_OnNewProject);
            NewLedEntityCommand = new Command(_OnNewLedEntity, () => _project != null);
            AddAudioCommand = new Command(_OnAddAudio, () => _project != null);

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        private void _OnNewProject()
        {
            View.SingleTextDialog _newProjectDialog = new View.SingleTextDialog();
            NewProjectDialogVM _newProjectDialogVM = new NewProjectDialogVM();

            App.Instance.WindowService.ShowNewWindow(_newProjectDialog, _newProjectDialogVM);

            if (_newProjectDialogVM.DialogResult)
                Project = new Model.Project(_newProjectDialogVM.ProjectName);
        }

        private void _OnNewLedEntity()
        {
            Project.LedEntities.Add(new Model.LedEntity());

            Views.CRUDs.LedEntityCRUD ledEntityCreationView = new Views.CRUDs.LedEntityCRUD();
            LedEntityCRUDVM ledEntityEditVM = new LedEntityCRUDVM(Project.LedEntities.Last());

            Views.Controls.CRUDs.LedEntityGroupProperties _ledGroupProperties = new Views.Controls.CRUDs.LedEntityGroupProperties();
            ledEntityCreationView.InnerGrid.Children.Add(_ledGroupProperties);

            Views.Controls.LedEntityOverview _ledEntity = new Views.Controls.LedEntityOverview();
            Grid.SetColumn(_ledEntity, 1);
            ledEntityCreationView.Grid.Children.Add(_ledEntity);

            App.Instance.WindowService.ShowNewWindow(ledEntityCreationView, ledEntityEditVM);

            LedEntities.Add(new LedEntitySelectVM(ledEntityEditVM.LedEntity));            
        }

        //private void OnSelectedLedEntity(object sender, EventArgs e)
        //{
        //    //LedEntitySelectViewModel test = new LedEntitySelectViewModel(((LedEntityViewModel)sender).LedEntity);
        //    //_ledEntityView.DataContext = test;
        //    _currentLedEntity = (LedEntityBaseVM)sender;
        //    _ledEntityView.DataContext = (LedEntityBaseVM)sender;         
        //}

        private void _OnAddAudio()
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
                    _LedEntityView.DataContext = (sender as LedEntityBaseVM);
                    break;
                default:
                    break;
            }    
        }
    }
}