using Led.Controller;
using Led.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Led.ViewModels
{
    public class AudioUserControlVM : INPC
    {
        private AudioPlayer player;

        public bool IsPlaying => player.IsPlaying;

        private ImageSource _waveform;
        public ImageSource Waveform
        {
            get => _waveform;
            private set
            {
                _waveform = value;
                RaisePropertyChanged();
            }
        }

        private double _progress = 0.0;
        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                RaisePropertyChanged();
            }
        }

        public double MaxVolume { get => 100; }
        public double Volume
        {
            get => player.Volume * MaxVolume;
            set
            {
                player.Volume = (float)(value / MaxVolume);
                RaisePropertyChanged();
            }
        }

        private TimeSpan _currentTime = TimeSpan.Zero;
        public string CurrentTime
        {
            get => _currentTime.ToDisplayString();
            set => SetCurrentTime(TimeSpanHelper.FromDisplayString(value));
        }
        private void SetCurrentTime(TimeSpan value)
        {
            _currentTime = value;
            _progress = _currentTime.Ticks / (double)_length.Ticks;
            RaisePropertyChanged(nameof(CurrentTime));
            RaisePropertyChanged(nameof(Progress));
        }

        private TimeSpan _length = TimeSpan.Zero;
        public string Length
        {
            get => _length.ToDisplayString();
            private set => SetLength(TimeSpanHelper.FromDisplayString(value));
        }
        private void SetLength(TimeSpan value)
        {
            _length = value;
            RaisePropertyChanged(nameof(Length));
        }


        public AudioUserControlVM(string filePath)
        {
            player = new AudioPlayer(filePath);

            PlayPauseCommand = new Command(() =>
            {
                if (IsPlaying)
                {
                    player.Pause();
                    updateTimer.Stop();
                }
                else
                {
                    player.Play(TimeSpanHelper.FromDisplayString(CurrentTime));
                    updateTimer.Start();
                }
                RaisePropertyChanged(playPausePropertyNames);
            }, () => _canExecute);
            ChangeTimeCommand = new Command<double>(progress =>
            {
                Debug.WriteLine("clicked waveform");
                var time = TimeSpan.FromTicks((long)(player.Length.Ticks * progress));
                Debug.WriteLine("clicked time: " + time);
                if (!IsPlaying)
                {
                    // set current time because it's used to start playback after pause
                    SetCurrentTime(time);
                }
                player.ChangeTime(time);
            });

            var theLength = player.Length;
            SetLength(theLength);
            // TODO set waveform size dynamically
            Task.Run(() => Waveform = player.CreateWaveform(1080, 270).ToImageSource());

            updateTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(1000 / 30), // refresh with 30fps
                IsEnabled = false
            };
            updateTimer.Tick += new EventHandler(UpdateTimer_Tick);

            player.PlaybackFinished += (sender, args) =>
            {
                updateTimer.Stop();
                SetCurrentTime(player.CurrentTime);
                RaisePropertyChanged(playPausePropertyNames);
            };
        }

        private static readonly ImageSource playImage = Properties.Resources.IconPlay.ToImageSource();
        private static readonly ImageSource pauseImage = Properties.Resources.IconPause.ToImageSource();
        public ImageSource PlayPauseImage => IsPlaying ? pauseImage : playImage;
        public string PlayPauseToolTipValue => IsPlaying ? "Pause" : "Play";
        private readonly string[] playPausePropertyNames = new[] {
            nameof(PlayPauseImage),
            nameof(PlayPauseToolTipValue)
        };

        public ICommand PlayPauseCommand { get; }
        public ICommand ChangeTimeCommand { get; }
        public ICommand LoadWaveformCommand { get; }

        private bool _canExecute = true;

        private DispatcherTimer updateTimer;
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            SetCurrentTime(player.CurrentTime);
        }

    }
}
