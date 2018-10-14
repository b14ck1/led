using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Led.Model
{
    [JsonObject]
    public class ImageInfo
    {
        [JsonProperty]
        public string Path { get; set; }

        [JsonProperty]
        public Size Size;

        public ImageInfo()
        {
            Path = "";
            Size = new Size();
        }        
    }
}
