using Led.Controller;
using NAudio.Wave;
using Newtonsoft.Json;
using System;

namespace Led.Model
{
    [JsonObject]
    public class AudioProperty
    {
        [JsonProperty]
        public TimeSpan Length { get; }

        [JsonProperty]
        public int Frames { get; }
        
        public string AudioName
        {
            get
            {
                string[] res = FilePath.Split('\\');
                return res[res.Length - 1].Split('.')[0];
            }
        }            

        [JsonProperty]
        public string FilePath { get; }
        
        public AudioProperty(int frames)
        {
            Frames = frames;
            Length = TimeSpan.FromMilliseconds(Frames / Defines.FramesPerSecond * 1000);
        }

        [JsonConstructor]
        public AudioProperty(string filePath)
        {
            FilePath = filePath;

            AudioFileReader audioFile = new AudioFileReader(FilePath);
            Length = audioFile.TotalTime;
            audioFile.Dispose();

            Frames = (int)(Length.TotalMilliseconds * Defines.FramesPerSecond/1000);
        }
    }
}