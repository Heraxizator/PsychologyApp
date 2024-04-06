
using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Models
{
    public class Audio
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? File { get; set; }
        public bool IsPlaying { get; set; }
        public IAudioPlayer? AudioPlayer { get; set; }
        public ICommand? ClickCommand { get; set; }
    }
}
