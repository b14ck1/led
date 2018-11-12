using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    class YesNoDialogVM : DialogVM
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


        private string _textToShow;
        public string TextToShow
        {
            get => _textToShow;
            set
            {
                if (_textToShow != value)
                {
                    _textToShow = value;
                    RaisePropertyChanged(nameof(TextToShow));
                }
            }
        }

        public Command OkCommand { get; set; }
        public Command AbortCommand { get; set; }
        public Command CloseWindowCommand { get; set; }

        public YesNoDialogVM(string dialogTitle = "Neues Projekt anlegen", string textToShow = "Bitte Namen eingeben:")
        {
            DialogTitle = dialogTitle;
            TextToShow = textToShow;
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
