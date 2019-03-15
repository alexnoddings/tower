using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using Tower.Core.Services;

namespace Tower.Services.Spotify
{
    public class SpotifyWebService : BindableService, ISpotifyService
    {
        private const int AccessTokenExpiredErrorStatus = 0x191;
        private const int MaxBackOff = 2048;
        private const string RedirectUri = "http://localhost:8090";
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(75);

        private readonly AuthorizationCodeAuth _authCodeAuth;
        private readonly SpotifyLocalService _localService;
        private SpotifyWebAPI _api;
        private string _refreshToken = "";

        private DateTime _songStartTime;
        private DateTime _songEndTime;
        
        private bool IsConnected { get; set; }

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
        private async Task TransferPlaybackToThisDeviceAsync()
        {
            string deviceId = (await _api.GetDevicesAsync()).Devices.First(d => d.Name == Environment.MachineName).Id;
            await _api.TransferPlaybackAsync(deviceId);
        }

        public async Task PreviousTrackAsync()
        {
            double progressMs = (DateTime.Now - _songStartTime).TotalMilliseconds;
            if (progressMs > 5000)
            {
                await _api.SeekPlaybackAsync(0);
                _songStartTime += TimeSpan.FromMilliseconds(progressMs);
                _songEndTime += TimeSpan.FromMilliseconds(progressMs);
            }
            else
                await _api.SkipPlaybackToPreviousAsync();
        }

        public async Task NextTrackAsync() => await _api.SkipPlaybackToNextAsync();

        public async Task PausePlaybackAsync() => await _api.PausePlaybackAsync();

        public async Task ResumePlaybackAsync(bool forceThisDevice = false, int positionMs = 0)
        {
            if (forceThisDevice)
                await TransferPlaybackToThisDeviceAsync();
            await _api.ResumePlaybackAsync("", "", null, "", positionMs);
        }
        #endregion

        public SpotifyWebService()
        {
            string clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            if (clientId == null) throw new ApplicationException("Environment variable SPOTIFY_CLIENT_ID not set");
            string secretId = Environment.GetEnvironmentVariable("SPOTIFY_SECRET_ID");
            if (secretId == null) throw new ApplicationException("Environment variable SPOTIFY_SECRET_ID not set");

            _authCodeAuth = new AuthorizationCodeAuth(clientId, secretId, RedirectUri, RedirectUri,
                Scope.UserModifyPlaybackState | Scope.UserReadPlaybackState | Scope.UserReadCurrentlyPlaying);
            _authCodeAuth.AuthReceived += AuthCodeAuth_AuthReceived;
            _authCodeAuth.Start();
            _authCodeAuth.OpenBrowser();
            
            _localService = new SpotifyLocalService();

            // Begin checking in the background
            Task.Factory.StartNew(async () => await CheckForUpdatesAsync());
            var updateTimerThread = new DispatcherTimer(DispatcherPriority.Normal) { Interval = UpdateInterval };
            updateTimerThread.Tick += async (_, __) => await CheckForUpdatesAsync();
            updateTimerThread.Start();

            // Tokens expire automatically after 60 mins. Try to auto refresh before then.
            var refreshTimerThread = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMinutes(59.5)
            };
            refreshTimerThread.Tick += async (_, __) => await RefreshApiAsync();
            refreshTimerThread.Start();

            AlbumArtUrl = "https://developer.spotify.com/assets/branding-guidelines/icon3@2x.png";
            TrackTitle = "Loading";
            TrackArtist = "Waiting for Spotify to connect...";
            PercentPlayed = 0;
            _songStartTime = DateTime.Now;
            _songEndTime = DateTime.Now;
        }

        private async void AuthCodeAuth_AuthReceived(object sender, AuthorizationCode payload)
        {
            var authcodeAuth = (AuthorizationCodeAuth)sender;

            Token token = await authcodeAuth.ExchangeCode(payload.Code);
            _refreshToken = token.RefreshToken;
            _api = new SpotifyWebAPI
            {
                AccessToken = token.AccessToken,
                RetryAfter = 250,
                RetryTimes = 10,
                TokenType = token.TokenType,
                UseAuth = true,
                UseAutoRetry = true
            };
            IsConnected = true;
        }

        private async Task CheckForUpdatesAsync()
        {
            if (!IsConnected || !_localService.IsConnected || _localService.TrackTitle == null || _localService.TrackArtist == null) return;

            if (IsPlaying)
            {
                double endMs = (_songEndTime - _songStartTime).TotalMilliseconds;
                double currentMs = (DateTime.Now - _songStartTime).TotalMilliseconds;
                PercentPlayed = Math.Min((currentMs / endMs) * 100, 100);
            }

            var backOffMs = 25;
            // Wait for a discrepancy between what the local spotify process says and the remote spotify API says
            // Local service track title may not always be 100% accurate with tracks, so check for contains instead
            while (IsPlaying != _localService.IsPlaying ||
                !TrackTitle.Contains(_localService.TrackTitle) ||
                !TrackArtist.Contains(_localService.TrackArtist))
            {
                PlaybackContext context = await GetPlaybackContextAsync();
                if (context == null)
                {
                    backOffMs = Math.Min(backOffMs * 2, MaxBackOff);
                    Thread.Sleep(backOffMs);
                    continue;
                }

                if (context?.Item == null)
                {
                    backOffMs = Math.Min(backOffMs * 2, MaxBackOff);
                    Thread.Sleep(backOffMs);
                    continue;
                }

                string contextTrackTitle = context.Item.Name;
                string contextTrackArtists = string.Join(", ", context.Item.Artists.Select(a => a.Name));

                // Local has received updates, remote hasn't. Need to wait for remote here.
                if (IsPlaying == context.IsPlaying && TrackTitle == contextTrackTitle &&
                    TrackArtist == contextTrackArtists)
                {
                    backOffMs = Math.Min(backOffMs * 2, MaxBackOff);
                    Thread.Sleep(backOffMs);
                    continue;
                }

                _songStartTime = DateTime.Now - TimeSpan.FromMilliseconds(context.ProgressMs);
                _songEndTime = _songStartTime + TimeSpan.FromMilliseconds(context.Item.DurationMs);
                IsPlaying = context.IsPlaying;
                TrackTitle = contextTrackTitle;
                TrackArtist = contextTrackArtists;
                AlbumArtUrl = context.Item.Album.Images.First().Url;
                break;
            }
        }

        private async Task<PlaybackContext> GetPlaybackContextAsync()
        {
            PlaybackContext ctx = await _api.GetPlaybackAsync();
            if (ctx.Error == null) return ctx;
            if (ctx.Error != null && ctx.Error.Status == AccessTokenExpiredErrorStatus)
                await RefreshApiAsync();
            // Other error = just ignore this update
            return null;
        }

        private async Task RefreshApiAsync()
        {
            Token token = null;
            try
            {
                token = await _authCodeAuth.RefreshToken(_refreshToken);
            }
            catch (System.Net.Http.HttpRequestException)
            {

            }

            if (token == null) return;
            
            _refreshToken = token.RefreshToken ?? _refreshToken;
            _api.AccessToken = token.AccessToken;
        }
}
}
