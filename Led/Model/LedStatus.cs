using Newtonsoft.Json;
using System.Windows.Media;

namespace Led.Model
{
    [JsonObject]
    class LedStatus : INPC
    {
        [JsonProperty]
        private byte _ledBusID;
        public byte LedBusID
        {
            get => _ledBusID;
            set
            {
                if (_ledBusID != value)
                {
                    _ledBusID = value;
                    RaisePropertyChanged("LedBusID");
                }
            }
        }

        [JsonProperty]
        private byte _ledGroupID;
        public byte LedGroupID
        {
            get => _ledGroupID;
            set
            {
                if (_ledGroupID != value)
                {
                    _ledGroupID = value;
                    RaisePropertyChanged("LedGroupID");
                }
            }
        }

        [JsonProperty]
        private ushort _ledID;
        public ushort LedID
        {
            get => _ledID;
            set
            {
                if (_ledID != value)
                {
                    _ledID = value;
                    RaisePropertyChanged("LedID");
                }
            }
        }

        [JsonProperty]
        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    RaisePropertyChanged("Color");                }
            }

        }
        public LedStatus()
        {

        }
    }
}
