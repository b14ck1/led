using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Led.ViewModels;

namespace Led.Services
{
    public class WindowService
    {
        private Dictionary<object, Window> _WindowRegistry;

        private void _AddWindowToRegistry(object dataContext, Window window)
        {
            _WindowRegistry.Add(dataContext, window);
        }

        public WindowService()
        {
            _WindowRegistry = new Dictionary<object, Window>();
        }

        public void ShowNewWindow(Window window, object dataContext = null, bool dialog = true)
        {
            window.DataContext = dataContext;
            _AddWindowToRegistry(dataContext, window);

            if (dialog)
                window.ShowDialog();
            else
                window.Show();
        }

        public void CloseWindow(object dataContext)
        {
            try
            {
                _WindowRegistry[dataContext].Close();
            }
            catch (Exception)
            {
                Debug.Print(ToString() + ": Window was closing so no need to close it again.");
            }

            _WindowRegistry.Remove(dataContext);
        }
    }
}
