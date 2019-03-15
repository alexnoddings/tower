using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower.Core.Services
{
    public interface ISpotifyService
    {
        bool IsPlaying { get; }
        string TrackTitle { get; }
        string TrackArtist { get; }
        string AlbumArtUrl { get; }
        double PercentPlayed { get; }

        event EventHandler PlaybackStateModified;

        Task PreviousTrackAsync();
        Task NextTrackAsync();
        Task ResumePlaybackAsync(bool forceThisDevice = false, int positionMs = 0);
        Task PausePlaybackAsync();
    }
}
