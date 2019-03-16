using System;
using System.Threading.Tasks;
using Tower.Core.Services;

namespace Tower.Application.DesignTimeServices
{
    internal class DesignTimeSpotifyService : BindableService, ISpotifyService
    {
        #region ISpotifyService Properties
        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                if (HandlePropertyChange(ref _isPlaying, value))
                    PlaybackStateModified?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _trackTitle;
        public string TrackTitle
        {
            get => _trackTitle;
            private set
            {
                if (HandlePropertyChange(ref _trackTitle, value))
                    PlaybackStateModified?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _trackArtist;
        public string TrackArtist
        {
            get => _trackArtist;
            private set
            {
                if (HandlePropertyChange(ref _trackArtist, value))
                    PlaybackStateModified?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _albumArtUrl;
        public string AlbumArtUrl
        {
            get => _albumArtUrl;
            private set
            {
                if (HandlePropertyChange(ref _albumArtUrl, value))
                    PlaybackStateModified?.Invoke(this, EventArgs.Empty);
            }
        }

        private double _percentPlayed;
        public double PercentPlayed
        {
            get => _percentPlayed;
            private set
            {
                if (HandlePropertyChange(ref _percentPlayed, value))
                    PlaybackStateModified?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler PlaybackStateModified;
        #endregion

        #region ISpotifyService Methods
        public Task PreviousTrackAsync()
        {
            throw new NotImplementedException();
        }

        public Task NextTrackAsync()
        {
            throw new NotImplementedException();
        }

        public Task ResumePlaybackAsync(bool forceThisDevice = false, int positionMs = 0)
        {
            throw new NotImplementedException();
        }

        public Task PausePlaybackAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

        public DesignTimeSpotifyService()
        {
            IsPlaying = true;
            // Shorter Title & Artist
            TrackTitle = "Holland, 1945";
            TrackArtist = "Neutral Milk Hotel";
            // Longer Title & Artist
            // TrackTitle = "Sgt. Pepper's Lonely Hearts Club Band - Remastered 2015";
            // TrackArtist = "King Gizzard & The Lizard Wizard";
            AlbumArtUrl = "https://i.scdn.co/image/4bd59c0b2ef97a9225600b4d5d5e7b45395ed9ad";
            PercentPlayed = 66;
        }
    }
}
