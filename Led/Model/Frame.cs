using Newtonsoft.Json;
using System.Collections.Generic;

namespace Led.Model
{
    [JsonObject]
    class Frame : INPC
    {
        //[JsonProperty]
        //private List<IEffect> _effects;
        //public List<IEffect> Effects
        //{
        //    get => _effects;
        //    set
        //    {
        //        if (_effects != value)
        //        {
        //            _effects = value;
        //            RaisePropertyChanged("Effects");
        //        }
        //    }
        //}

        [JsonProperty]
        private List<LedChangeData> _ledChanges;
        public List<LedChangeData> LedChanges
        {
            get => _ledChanges;
            set
            {
                if (_ledChanges != value)
                {
                    _ledChanges = value;
                    RaisePropertyChanged("LedChanges");
                }
            }
        }

        public Frame()
        {

        }
    }
}
