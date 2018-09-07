using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    class DialogVM : INPC
    {
        public bool DialogResult;

        public void OnOkCommand()
        {
            DialogResult = true;
            App.Instance.WindowService.CloseWindow(this);
        }

        public void OnAbortCommand()
        {
            DialogResult = false;
            App.Instance.WindowService.CloseWindow(this);
        }
    }
}
