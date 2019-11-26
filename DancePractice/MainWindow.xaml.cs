﻿using System;
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
using WPFMediaKit.DirectShow.Controls;

namespace DancePractice
{
    public partial class MainWindow : Window
    {
        string VIDEO_ROOT = Path.Combine(System.Environment.CurrentDirectory, "Video");

        public MainWindow()
        {
            InitializeComponent();

            listDance.ItemsSource = GetData();

            if (MultimediaUtil.VideoInputNames.Count() > 0)
            {
                vce.VideoCaptureSource = MultimediaUtil.VideoInputNames[0];
                vce.Play();
            }
            else
            {
                MessageBox.Show("未发现摄像头");
            }

            player.SizeChanged += delegate
            {
                AdjustVideoCaptureElementSize();
            };
        }

        private List<string> GetData()
        {
            List<string> result = new List<string>();
            var files = new DirectoryInfo(VIDEO_ROOT).GetFiles("*.mp4");
            foreach (var file in files)
            {
                if (file.Name.Contains("_"))
                {
                    result.Add(file.Name.Split('_')[0]);
                }
                else
                {
                    result.Add(file.Name.Split('.')[0]);
                }
            }
            result = result.Distinct().OrderBy(p => p).ToList();

            return result;
        }

        private void ListDance_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlayDanceVideo();

            // 选择已选中的项，不会触发此事件
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            // player.Stop();
            // player.Play();

            PlayDanceVideo();
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

        private void PlayDanceVideo()
        {
            var danceName = (string)listDance.SelectedItem;
            string videoPath = Path.Combine(VIDEO_ROOT, danceName + ".mp4");
            if ((radType2.IsChecked.HasValue && radType2.IsChecked.Value && File.Exists(Path.Combine(VIDEO_ROOT, danceName + "_0.mp4")) || !File.Exists(videoPath)))
            {
                videoPath = Path.Combine(VIDEO_ROOT, danceName + "_0.mp4");
            }

            int angle = 0;
            if (danceName.Contains("-"))
            {
                angle = int.Parse(danceName.Split('-')[1]);
            }
            rt.Angle = angle;

            player.Source = new Uri(videoPath);
            player.Play();
            // player.SpeedRatio = sliderSpeed.Value;
            GroupSpeed_Checked(null, null);
        }

        private void AdjustVideoCaptureElementSize()
        {
            vce.Height = player.ActualHeight * 0.3;
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            txtMsg.Text = e.ErrorException.Message;
            // MessageBox.Show(e.ErrorException.Message);
        }

        private void GroupSpeed_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                double speed = 1d;
                if (rad25.IsChecked.Value)
                {
                    speed = 0.25;
                }
                else if (rad50.IsChecked.Value)
                {
                    speed = 0.5;
                }
                else if (rad75.IsChecked.Value)
                {
                    speed = 0.75;
                }

                txtSpeed.Text = speed.ToString("0.00") + "x";
                player.SpeedRatio = speed;
            }
            catch { }
        }
    }
}
