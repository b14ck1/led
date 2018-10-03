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

        public NewProjectDialogVM(string dialogTitle = "Neues Projekt anlegen", string textToEnter = "Bitte Namen eingeben:")
        {
            DialogTitle = dialogTitle;
            TextToEnter = textToEnter;
            OkCommand = new Command(_OnOkCommand);
            AbortCommand = new Command(_OnAbortCommand);
            CloseWindowCommand = new Command(_OnCloseWindowCommand);
        }

        private void _OnCloseWindowCommand()
        {
            App.Instance.WindowService.CloseWindow(this);
        }
    }
}
