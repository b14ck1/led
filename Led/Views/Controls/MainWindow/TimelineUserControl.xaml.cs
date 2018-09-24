using Led.Model.Effect;
using Led.Utility.Timeline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for TimelineUserControl.xaml
    /// </summary>
    public partial class TimelineUserControl : UserControl
    {
        public TimelineUserControl()
        {
            InitializeComponent();

            TimeLine2.StartDate = (DateTime)new UshortDateConverter().Convert((ushort)0, null, null, null);
        }

        private void Slider_Scale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeLine2.UnitSize = Slider_Scale.Value;
        }
    }
}
