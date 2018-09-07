using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ImageTrackBar : UserControl
    {
        public ImageTrackBar()
        {
            InitializeComponent();
        }

        private double? clickedProgress;
        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickCommand != null)
            {
                var mouseX = Mouse.GetPosition(PART_Grid).X;
                // save the clicked progress instead of setting Progress directly since Progress might be overriden
                // before MouseLeftButtonUp is triggered
                clickedProgress = mouseX / PART_Grid.ActualWidth;
            }
        }

        private void RepeatButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // check if clicked progress has been set while holding down the mouse button (just in case)
            // and set it back to null afterwards.
            if (clickedProgress != null)
            {
                ClickCommand?.Execute(clickedProgress);
                Progress = clickedProgress.Value;
                clickedProgress = null;
            }
        }



        public ImageSource BackgroundImage
        {
            get { return (ImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundImageProperty =
            DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(ImageTrackBar), new PropertyMetadata(null));



        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ImageTrackBar), new PropertyMetadata(
                0.0,
                new PropertyChangedCallback(OnProgressChanged),
                null
            ), null);

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ImageTrackBar)d;
            c.PART_ProgressRect.Width = c.PART_Grid.ActualWidth * c.Progress;
        }



        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ImageTrackBar), null);



        public ICommand ClickCommand
        {
            get { return (ICommand)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(ImageTrackBar), new PropertyMetadata(null));

    }
}
