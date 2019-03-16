using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Tower.Core.Services;

namespace Tower.Application.ViewModel
{
    public class MusicViewModel : ViewModelBase
    {
        public ISpotifyService SpotifyService { get; }

        public RelayCommand PreviousCommand { get; }
        public RelayCommand TogglePlaybackCommand { get; }
        public RelayCommand NextCommand { get; }

        public MusicViewModel(ISpotifyService spotifyService)
        {
            SpotifyService = spotifyService;

            PreviousCommand = new RelayCommand(async () => await SpotifyService.PreviousTrackAsync());
            TogglePlaybackCommand = new RelayCommand(async () => await TogglePlaybackAsync());
            NextCommand = new RelayCommand(async () => await SpotifyService.NextTrackAsync());
        }

        private async Task TogglePlaybackAsync()
        {
            if (SpotifyService.IsPlaying)
                await SpotifyService.PausePlaybackAsync();
            else
                await SpotifyService.ResumePlaybackAsync();
        }
    }
}
