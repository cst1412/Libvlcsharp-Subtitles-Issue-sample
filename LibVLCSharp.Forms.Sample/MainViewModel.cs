using LibVLCSharp.Shared;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LibVLCSharp.Forms.Sample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            Initialize();
        }

        private LibVLC LibVLC { get; set; }

        private MediaPlayer _mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => Set(nameof(MediaPlayer), ref _mediaPlayer, value);
        }

        private bool IsLoaded { get; set; }
        private bool IsVideoViewInitialized { get; set; }

        private void Set<T>(string propertyName, ref T field, T value)
        {
            if (field == null && value != null || field != null && !field.Equals(value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Initialize()
        {
            Core.Initialize();

            LibVLC = new LibVLC(enableDebugLogs: true);
            var media = new Media(LibVLC, new Uri("http://raw.githubusercontent.com/cst1412/Libvlcsharp-Subtitles-Issue-sample/master/encoded_sample.mkv"));

            MediaPlayer = new MediaPlayer(LibVLC)
            {
                Media = media
            };

            media.Dispose();
        }

        public void OnAppearing()
        {
            IsLoaded = true;
            Play();
        }

        internal void OnDisappearing()
        {
            MediaPlayer.Dispose();
            LibVLC.Dispose();
        }

        public void OnVideoViewInitialized()
        {
            IsVideoViewInitialized = true;
            Play();
        }

        private void Play()
        {
            if (IsLoaded && IsVideoViewInitialized)
            {
                MediaPlayer.Play();
                MediaTrack? subtitleTrack = this.MediaPlayer.Media.Tracks.FirstOrDefault(x => x.TrackType == TrackType.Text);
                if(subtitleTrack.HasValue)
                {
                    this.MediaPlayer.SetSpu(subtitleTrack.Value.Id);
                }
            }
        }
    }
}
