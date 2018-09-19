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
        public ObservableCollection<EffectBase> t2Data = new ObservableCollection<EffectBase>();
        public ushort TotalFrames = 5000; // TODO item can be moved passed this value if you move your mouse fast...

        public TimelineUserControl()
        {
            InitializeComponent();

            var tmp1 = new EffectBlinkColor()
            {
                StartFrame = 3,
                EndFrame = 18,
                //Name = "Temp 1"
            };
            var tmp2 = new EffectFadeColor()
            {
                StartFrame = 18,
                EndFrame = 33,
                //Name = "Temp 2"
            };

            t2Data.Add(tmp1);
            t2Data.Add(tmp2);

            TimeLine2.StartDate = DateTime.MinValue.AddMinutes(1); // TODO added 1 so it isn't 00:00, the tool doesn't like that

            TimeLine2.Items = t2Data;
            TimeLine2.TotalFrames = TotalFrames;

        }

        // TODO remove later
        // TODO not working
        private Random random = new Random();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var startR = random.Next(0, 150);
            var endR = startR + random.Next(1, 100);
            var randomItem = new EffectBlinkColor()
            {
                StartFrame = (ushort)startR,
                EndFrame = (ushort)endR,
                //Name = "random"
            };

            TimeLine2.Items.Add(randomItem);
        }


        private void Slider_Scale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeLine2.UnitSize = Slider_Scale.Value;
        }
    }
}
