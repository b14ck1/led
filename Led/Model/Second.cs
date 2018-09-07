using Newtonsoft.Json;
using System.Collections.Generic;

namespace Led.Model
{
    [JsonObject]
    class Second : INPC
    {
        [JsonProperty]
        private Frame[] _frames;
        public Frame[] Frames
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
        private List<LedStatus> _ledEntityStatus;
        public List<LedStatus> LedEntityStatus
        {
            get => _ledEntityStatus;
            set
            {
                if (_ledEntityStatus != value)
                {
                    _ledEntityStatus = value;
                    RaisePropertyChanged("LedEntityStatus");
                }
            }
        }

        public Second()
        {

        }
    }
}
