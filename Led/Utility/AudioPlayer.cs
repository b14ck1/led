using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveFormRendererLib;

namespace Led.Controller
{
    public sealed class AudioPlayer : IDisposable
    {
        private WaveOutEvent _OutputDevice;
        private AudioFileReader _AudioFile;
        private readonly string _FilePath;

        /// <summary>
        /// Current playback position of the audio file.
        /// </summary>
        public TimeSpan CurrentTime => _AudioFile.CurrentTime;
        /// <summary>
        /// Total length of the audio file.
        /// </summary>
        public readonly TimeSpan Length;
        public bool IsPlaying => _OutputDevice.PlaybackState.Equals(PlaybackState.Playing);

        public float Volume
        {
            get => _OutputDevice.Volume;
            set => _OutputDevice.Volume = value;
        }

        /// <summary>
        /// Event called when an audio file has finished playing "naturally".
        /// </summary>
        public event EventHandler PlaybackFinished;

        public AudioPlayer(string audioFilePath)
        {
            _FilePath = audioFilePath;
            _AudioFile = new AudioFileReader(audioFilePath);
            Length = _AudioFile.TotalTime;
            _OutputDevice = new WaveOutEvent();
            _OutputDevice.PlaybackStopped += OnPlaybackStopped;
            _OutputDevice.Init(_AudioFile);
        }

        public Image CreateWaveform(int width, int height)
        {
            var maxPeakProvider = new MaxPeakProvider();

            var pen = Pens.Black;

            // Settings
            var myRendererSettings = new StandardWaveFormRendererSettings
            {
                Width = width,
                TopHeight = height / 2,
                BottomHeight = height / 2,
                BackgroundColor = Color.Transparent,
                TopPeakPen = pen,
                BottomPeakPen = pen
            };

            // create WaveFormRenderer
            var renderer = new WaveFormRenderer();
            var image = renderer.Render(_FilePath, maxPeakProvider, myRendererSettings);
            return image;
        }

        /// <summary>
        /// Plays the audio file from the give position or sets it for paused files.
        /// </summary>
        /// <param name="time">TimeSpan to start playback at</param>
        /// <param name="volume">Playback volume, defaults to 1f (100%) if null</param>
        public void Play(TimeSpan time)
        {
            var pausedTime = _AudioFile.CurrentTime;
            Debug.WriteLine($"paused timeSpan: {pausedTime}, timeSpan to start: {time}");
            // don't change time if it didn't change, or playback is almost finished (-1 seconds to fix stopped event delay)
            if (!pausedTime.Equals(time) && time.CompareTo(_AudioFile.TotalTime.Subtract(TimeSpan.FromSeconds(1))) < 0)
            {
                ChangeTime(time);
            }
            Play();
        }

        public void Pause()
        {
            _OutputDevice.Pause();
            Debug.WriteLine("PAUSE: " + CurrentTime);
        }

        /// <summary>
        /// Changes the <see cref="CurrentTime"/> to the given TimeSpan.
        /// The current PlaybackState won't be changed by this.
        /// </summary>
        /// <param name="newTime">TimeSpan to start playback at</param>
        public void ChangeTime(TimeSpan newTime)
        {
            switch (_OutputDevice.PlaybackState)
            {
                case PlaybackState.Playing:
                    // stop to clear buffer
                    Stop();
                    _AudioFile.CurrentTime = newTime;
                    Play();
                    break;
                case PlaybackState.Paused:
                    // stop to clear buffer
                    Stop();
                    _AudioFile.CurrentTime = newTime;
                    break;
                case PlaybackState.Stopped:
                    _AudioFile.CurrentTime = newTime;
                    break;
            }
        }

        private void Play()
        {
            _OutputDevice.Play();
            Debug.WriteLine("PLAY: " + CurrentTime);
        }


        private void Stop()
        {
            _OutputDevice.Stop();
            Debug.WriteLine("STOP: " + CurrentTime);
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            Debug.WriteLine("STOP (event): " + CurrentTime);
            // reset playback position if playback has "normally" stopped
            if (CurrentTime >= Length)
            {
                _AudioFile.Position = 0;
                PlaybackFinished?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            _OutputDevice.Dispose();
            _AudioFile.Dispose();
        }
    }

}
