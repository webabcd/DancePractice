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
using System.IO;
using System.Windows.Threading;

namespace DancePractice
{
    public partial class MainWindow : Window
    {
        string VIDEO_ROOT = Path.Combine(System.Environment.CurrentDirectory, "Video");

        public MainWindow()
        {
            InitializeComponent();

            listDance.ItemsSource = GetData();
        }
        
        private List<string> GetData()
        {
            List<string> result = new List<string>();
            var files = new DirectoryInfo(VIDEO_ROOT).GetFiles("*.mp4");
            foreach (var file in files)
            {
                string name = file.Name.Split('_')[0];
                result.Add(name);
            }
            result = result.Distinct().ToList();

            return result;
        }

        private void ListDance_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 选择已选中的项，不会触发此事件
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            player.Stop();
            player.Play();
            player.SpeedRatio = sliderSpeed.Value;
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            txtSpeed.Text = sliderSpeed.Value.ToString("0.00") + "x";
        }

        private void SliderSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                txtSpeed.Text = sliderSpeed.Value.ToString("0.00") + "x";
                player.SpeedRatio = sliderSpeed.Value;
            }
            catch { }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var danceName = (string)listDance.SelectedItem;
            string videoPath = Path.Combine(VIDEO_ROOT, danceName + "_0.mp4");
            if (radType2.IsChecked.HasValue && radType2.IsChecked.Value)
            {
                videoPath = Path.Combine(VIDEO_ROOT, danceName + "_1.mp4");
            }

            player.Source = new Uri(videoPath);
            player.Play();
            player.SpeedRatio = sliderSpeed.Value;
        }
    }
}
