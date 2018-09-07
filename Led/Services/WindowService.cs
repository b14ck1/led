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
        private Dictionary<object, Window> _windowRegistry;

        private void _AddWindowToRegistry(object _dataContext, Window _window)
        {
            _windowRegistry.Add(_dataContext, _window);
        }

        public WindowService()
        {
            _windowRegistry = new Dictionary<object, Window>();
        }

        public void ShowNewWindow(Window _window, object _dataContext = null, bool _dialog = true)
        {
            _window.DataContext = _dataContext;
            _AddWindowToRegistry(_dataContext, _window);

            if (_dialog)
                _window.ShowDialog();
            else
                _window.Show();
        }

        public void CloseWindow(object _dataContext)
        {
            try
            {
                _windowRegistry[_dataContext].Close();
            }
            catch (Exception)
            {
                Debug.Print(ToString() + ": Window was closing so no need to close it again.");
            }

            _windowRegistry.Remove(_dataContext);
        }
    }
}
