using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

namespace Tower.Services.Spotify
{
    internal class SpotifyLocalService
    {
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(75);

        internal string TrackTitle { get; private set; }
        internal string TrackArtist { get; private set; }
        internal bool IsPlaying { get; private set; }
        internal bool IsConnected { get; private set; }

        private string _spotifyLastTitle = "";

        public SpotifyLocalService()
        {
            var timerThread = new DispatcherTimer(DispatcherPriority.Normal) { Interval = UpdateInterval };
            timerThread.Tick += (_, __) => Update();
            timerThread.Start();
            // Manually update once
            Update();
        }

        private void Update()
        {
            // Need to re-fetch every time to avoid inconsistency issues
            Process spotifyProcess = Process.GetProcessesByName("Spotify").FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero);
            if (spotifyProcess == null)
            {
                IsConnected = false;
                TrackTitle = "Waiting for Spotify to play";
                TrackArtist = "Spotify process with non-intptr window handle cannot be found";
                IsPlaying = false;
            }
            else
            {
                IsConnected = true;
                string spotifyWindowTitle = spotifyProcess.MainWindowTitle;

                _spotifyLastTitle = spotifyWindowTitle;
                if (spotifyWindowTitle == "Spotify" || spotifyWindowTitle == "Spotify Premium")
                {
                    IsPlaying = false;
                }
                else
                {
                    IsPlaying = true;
                    string[] split = _spotifyLastTitle.Split(new[] { " - " }, StringSplitOptions.None);
                    if (split.Length == 1)
                    {
                        TrackTitle = split[0];
                        TrackArtist = "N/A";
                    }
                    else
                    {
                        TrackTitle = split[1];
                        TrackArtist = split[0];
                    }
                }
            }
        }
    }
}
