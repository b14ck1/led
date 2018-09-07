using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    class NewProjectDialogVM : DialogVM
    {

        private string _dialogTitle;
        public string DialogTitle
        {
            get => _dialogTitle;
            set
            {
                if (_dialogTitle != value)
                {
                    _dialogTitle = value;
                    RaisePropertyChanged("DialogTitle");
                }
            }
        }


        private string _textToEnter;
        public string TextToEnter
        {
            get => _textToEnter;
            set
            {
                if (_textToEnter != value)
                {
                    _textToEnter = value;
                    RaisePropertyChanged("TextToEnter");
                }
            }
        }

        private string _projectName;
        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (_projectName != value)
                {
                    _projectName = value;
                    RaisePropertyChanged("ProjectName");
                }
            }
        }

        public Command OkCommand { get; set; }
        public Command AbortCommand { get; set; }
        public Command CloseWindowCommand { get; set; }

        public NewProjectDialogVM(string _dialogTitle = "Neues Projekt anlegen", string _textToEnter = "Bitte Namen eingeben:")
        {
            DialogTitle = _dialogTitle;
            TextToEnter = _textToEnter;
            OkCommand = new Command(OnOkCommand);
            AbortCommand = new Command(OnAbortCommand);
            CloseWindowCommand = new Command(OnCloseWindowCommand);
        }

        private void OnCloseWindowCommand()
        {
            App.Instance.WindowService.CloseWindow(this);
        }
    }
}
