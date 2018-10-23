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
    public class AudioUserControlVM : INPC, Interfaces.IParticipant
    {
        private AudioPlayer _Player;
        private Services.MediatorService _MediatorService;

        public bool IsPlaying => _Player.IsPlaying;

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
            get => _Player.Volume * MaxVolume;
            set
            {
                _Player.Volume = (float)(value / MaxVolume);
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
            _Player = new AudioPlayer(filePath);

            PlayPauseCommand = new Command(() =>
            {
                if (IsPlaying)
                {
                    _Player.Pause();
                    _SendMessage(MediatorMessages.AudioControlPlayPause, new MediatorMessageData.AudioControlPlayPauseData((long)(_Player.CurrentTime.TotalMilliseconds * Defines.FramesPerSecond / 1000), false));
                    _UpdateTimer.Stop();
                }
                else
                {
                    _SendMessage(MediatorMessages.EffectServiceRenderAll, null);
                    _Player.Play(TimeSpanHelper.FromDisplayString(CurrentTime));
                    _SendMessage(MediatorMessages.AudioControlPlayPause, new MediatorMessageData.AudioControlPlayPauseData((long)(_Player.CurrentTime.TotalMilliseconds * Defines.FramesPerSecond / 1000), true));
                    _UpdateTimer.Start();
                }
                RaisePropertyChanged(playPausePropertyNames);
            }, () => _CanExecute);
            ChangeTimeCommand = new Command<double>(progress =>
            {
                Debug.WriteLine("clicked waveform");
                var time = TimeSpan.FromTicks((long)(_Player.Length.Ticks * progress));
                Debug.WriteLine("clicked time: " + time);
                if (!IsPlaying)
                {
                    // set current time because it's used to start playback after pause
                    SetCurrentTime(time);
                }
                _Player.ChangeTime(time);
            });

            var theLength = _Player.Length;
            SetLength(theLength);
            // TODO set waveform size dynamically
            Task.Run(() => Waveform = _Player.CreateWaveform(1080, 270).ToImageSource());

            _UpdateTimer = new DispatcherTimer(DispatcherPriority.Send)
            {
                Interval = TimeSpan.FromMilliseconds(1000 / 30), // refresh with 30fps
                IsEnabled = false
            };
            _UpdateTimer.Tick += new EventHandler(UpdateTimer_Tick);

            _Player.PlaybackFinished += (sender, args) =>
            {
                _UpdateTimer.Stop();
                SetCurrentTime(_Player.CurrentTime);
                RaisePropertyChanged(playPausePropertyNames);
            };

            _MediatorService = App.Instance.MediatorService;
            _MediatorService.Register(this);
        }

        private static readonly ImageSource _PlayImage = Properties.Resources.IconPlay.ToImageSource();
        private static readonly ImageSource _PauseImage = Properties.Resources.IconPause.ToImageSource();
        public ImageSource PlayPauseImage => IsPlaying ? _PauseImage : _PlayImage;
        public string PlayPauseToolTipValue => IsPlaying ? "Pause" : "Play";
        private readonly string[] playPausePropertyNames = new[] {
            nameof(PlayPauseImage),
            nameof(PlayPauseToolTipValue)
        };

        public ICommand PlayPauseCommand { get; }
        public ICommand ChangeTimeCommand { get; }
        public ICommand LoadWaveformCommand { get; }

        private bool _CanExecute = true;

        private DispatcherTimer _UpdateTimer;
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            _SendMessage(MediatorMessages.AudioControlCurrentTick, new MediatorMessageData.AudioControlCurrentFrameData((long)(_Player.CurrentTime.TotalMilliseconds * Defines.FramesPerSecond / 1000)));
            SetCurrentTime(_Player.CurrentTime);
        }

        private void _SendMessage(MediatorMessages message, object data)
        {
            _MediatorService.BroadcastMessage(message, this, data);
        }

        public void RecieveMessage(MediatorMessages message, object sender, object data)
        {

        }

        public void Dispose()
        {
            _Player.Dispose();
            _MediatorService.Unregister(this);
        }
    }
}
