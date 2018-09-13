using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.ViewModels
{
    class DialogVM : INPC
    {
        public bool DialogResult { get; set; }

        protected void _OnOkCommand()
        {
            DialogResult = true;
            App.Instance.WindowService.CloseWindow(this);
        }

        protected void _OnAbortCommand()
        {
            DialogResult = false;
            App.Instance.WindowService.CloseWindow(this);
        }
    }
}
