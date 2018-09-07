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

        private Views.Controls.LedEntityOverview _ledEntityView;
        private Views.MainWindow _mainWindow;

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

        private LedEntityBaseVM _currentLedEntity;
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

        private Views.Controls.MainWindow.EffectProperties EffectView;
        private EffectBaseVM EffectViewModel;

        private Views.Controls.MainWindow.AudioUserControl _AudioUserControl;
        public AudioUserControlVM _audioUserControlVM;
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
            _mainWindow = mainWindow;
            _ledEntityView = ledEntity;
            ledEntity.DataContext = null;
            EffectView = effectView;

            _ledEntities = new ObservableCollection<LedEntityBaseVM>();
            
            EffectViewModel = new EffectBaseVM();            
            effectView.DataContext = EffectViewModel;
            _AudioUserControl = audioUserControl;
            audioUserControl.DataContext = AudioUserControlVM;

            NewProjectCommand = new Command(OnNewProject);
            NewLedEntityCommand = new Command(OnNewLedEntity, () => _project != null);
            AddAudioCommand = new Command(OnAddAudio, () => _project != null);

            _Mediator = App.Instance.MediatorService;
            _Mediator.Register(this);
        }

        private void OnNewProject()
        {
            View.SingleTextDialog _newProjectDialog = new View.SingleTextDialog();
            NewProjectDialogVM _newProjectDialogVM = new NewProjectDialogVM();

            App.Instance.WindowService.ShowNewWindow(_newProjectDialog, _newProjectDialogVM);

            if (_newProjectDialogVM.DialogResult)
                Project = new Model.Project(_newProjectDialogVM.ProjectName);
        }

        private void OnNewLedEntity()
        {
            Project.LedEntities.Add(new Model.LedEntity());

            Views.CRUDs.LedEntityCRUD _LedEntityCreationView = new Views.CRUDs.LedEntityCRUD();
            LedEntityCRUDVM _ledEntityEditVM = new LedEntityCRUDVM(Project.LedEntities.Last());

            Views.Controls.CRUDs.LedEntityGroupProperties _ledGroupProperties = new Views.Controls.CRUDs.LedEntityGroupProperties();
            _LedEntityCreationView.InnerGrid.Children.Add(_ledGroupProperties);

            Views.Controls.LedEntityOverview _ledEntity = new Views.Controls.LedEntityOverview();
            Grid.SetColumn(_ledEntity, 1);
            _LedEntityCreationView.Grid.Children.Add(_ledEntity);

            App.Instance.WindowService.ShowNewWindow(_LedEntityCreationView, _ledEntityEditVM);

            LedEntities.Add(new LedEntitySelectVM(_ledEntityEditVM.LedEntity));            
        }

        //private void OnSelectedLedEntity(object sender, EventArgs e)
        //{
        //    //LedEntitySelectViewModel test = new LedEntitySelectViewModel(((LedEntityViewModel)sender).LedEntity);
        //    //_ledEntityView.DataContext = test;
        //    _currentLedEntity = (LedEntityBaseVM)sender;
        //    _ledEntityView.DataContext = (LedEntityBaseVM)sender;         
        //}

        private void OnEditSelectedLeds(EditLedArgs e)
        {
            if (e.Edit)
            {
                LedEntitySelectVM temp = new LedEntitySelectVM(((LedEntityBaseVM)_ledEntityView.DataContext).LedEntity, e.SelectedLeds);
                _ledEntityView.DataContext = temp;
            }
            else
            {
                EffectViewModel.Leds = ((LedEntitySelectVM)_ledEntityView.DataContext).SelectedLeds;

                Model.LedChangeData test = new Model.LedChangeData(EffectViewModel.Leds, System.Windows.Media.Colors.Red, 0);
                _currentLedEntity.SetLedColor(test);

                //MouseCommandsBindings resetten
                //Unbedingt irgendwie reworken                
                //_mainWindow.Grid.Children.Remove(_ledEntityView);
                //_ledEntityView = new View.LedEntity();
                //System.Windows.Controls.Grid.SetColumn(_ledEntityView, 1);
                //System.Windows.Controls.Grid.SetRow(_ledEntityView, 1);
                //_mainWindow.Grid.Children.Add(_ledEntityView);
                
                _ledEntityView.DataContext = _currentLedEntity;
            }
        }

        private void OnAddAudio()
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
                case MediatorMessages.LedEntitySelected:
                    _currentLedEntity = (sender as LedEntityBaseVM);
                    _ledEntityView.DataContext = (sender as LedEntityBaseVM);
                    break;
                case MediatorMessages.EditedSelectedLeds:
                    OnEditSelectedLeds(data as EditLedArgs);
                    break;
                default:
                    break;
            }    
        }
    }
}