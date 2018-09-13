using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Services
{
    public class IOService
    {        
        public string OpenFileDialog(string filter = null)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog _Dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = filter
            };

            // Show open file dialog box
            _Dialog.ShowDialog();

            return _Dialog.FileName;
        }
    }
}
