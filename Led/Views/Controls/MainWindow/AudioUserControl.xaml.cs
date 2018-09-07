using Led.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Led.Views.Controls.MainWindow
{
    /// <summary>
    /// Interaction logic for AudioUserControl.xaml
    /// </summary>
    public partial class AudioUserControl : UserControl
    {
        public AudioUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO load waveform based on size?
        }

        private void VolumeSlider_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Slider s = (Slider)sender;
            double changeDelta = e.Delta * s.LargeChange / 120;
            var newValue = s.Value + changeDelta;
            if (newValue < 0)
            {
                s.Value = 0;
            }
            else if (newValue > s.Maximum)
            {
                s.Value = s.Maximum;
            }
            else
            {
                s.Value = newValue;
            }
        }
    }
}
