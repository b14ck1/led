using Newtonsoft.Json;

namespace Led.Model
{
    [JsonObject]
    class AudioProperty
    {
        [JsonProperty]
        private int _seconds;
        public int Seconds
        {
            get => _seconds;
            set
            {
                if (_seconds != value)
                {
                    _seconds = value;
                    RaisePropertyChanged("Seconds");
                }
            }
        }

        [JsonProperty]
        private int _frames;
        public int Frames
        {
            get => _frames;
            set
            {
                if (_frames != value)
                {
                    _frames = value;
                    RaisePropertyChanged("Frames");
                }
            }
        }

        [JsonProperty]
        private string _audioName;
        public string AudioName
        {
            get => _audioName;
            set
            {
                if (_audioName != value)
                {
                    _audioName = value;
                    RaisePropertyChanged("AudioName");
                }
            }
        }

        [JsonProperty]
        private string _filePath;
        /// <summary>
        /// Relative file path from the project directory
        /// </summary>
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    RaisePropertyChanged("FilePath");
                }
            }
        }

        public AudioProperty()
        {

        }
    }
}
