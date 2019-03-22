using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Tower.Application.DesignTimeServices;
using Tower.Application.Views;
using Tower.Core.Services;
using Tower.Services.BingBackground;

namespace Tower.Application.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public IBackgroundService BackgroundService { get; }
        public ITimeService TimeService { get; }
        private IBrightnessService _brightnessService;
        private ISpotifyService _spotifyService;

        private bool _isDayMode;
        public bool IsDayMode
        {
            get => _isDayMode;
            set => HandlePropertyChange(ref _isDayMode, value);
        }

        private bool _isAlarmControlVisible;
        public bool IsAlarmControlVisible
        {
            get => _isAlarmControlVisible;
            set => HandlePropertyChange(ref _isAlarmControlVisible, value);
        }

        private bool _isBackgroundSelectionVisible;
        public bool IsBackgroundSelectionVisible
        {
            get => _isBackgroundSelectionVisible;
            set => HandlePropertyChange(ref _isBackgroundSelectionVisible, value);
        }

        private bool _isSettingsVisible;
        public bool IsSettingsVisible
        {
            get => _isSettingsVisible;
            set => HandlePropertyChange(ref _isSettingsVisible, value);
        }

        private UserControl _dayView;
        private UserControl _nightView;
        private UserControl _musicView;

        private UserControl _activeUserControl;
        public UserControl ActiveUserControl
        {
            get => _activeUserControl;
            set => HandlePropertyChange(ref _activeUserControl, value);
        }

        public RelayCommand ToggleSettingsCommand { get; }
        public RelayCommand ToggleDayModeCommand { get; }
        public RelayCommand OpenAlarmCommand { get; }
        public RelayCommand CloseAlarmCommand { get; }
        public RelayCommand OpenBackgroundsCommand { get; }
        public RelayCommand CloseBackgroundsCommand { get; }
        public RelayCommand CloseApplicationCommand { get; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IBackgroundService backgroundService, ITimeService timeService, IBrightnessService brightnessService, ISpotifyService spotifyService)
        {
            if (IsInDesignMode)
            {
                IsAlarmControlVisible = false;
                IsBackgroundSelectionVisible = false;
                IsSettingsVisible = true;
            }
            else
            {
                IsAlarmControlVisible = false;
                IsBackgroundSelectionVisible = false;
                IsSettingsVisible = false;
            }
            
            _dayView = new DayView();
            _nightView = new NightView();
            _musicView = new MusicView();
            IsDayMode = true;

            BackgroundService = backgroundService;
            TimeService = timeService;
            _brightnessService = brightnessService;
            _spotifyService = spotifyService;

            UpdateActiveUserControl();
            
            TimeService.AlarmTriggered += async (_, __) =>
            {
                Task resumePlaybackTask = _spotifyService.ResumePlaybackAsync(true, 1);
                IsDayMode = true;
                UpdateActiveUserControl();
                await resumePlaybackTask;
            };
            _spotifyService.PlaybackStateModified += (_, __) => UpdateActiveUserControl();

            ToggleSettingsCommand = new RelayCommand(() => IsSettingsVisible = !IsSettingsVisible);
            ToggleDayModeCommand = new RelayCommand(() => { ToggleDayMode(); });
            OpenAlarmCommand = new RelayCommand(() => { OpenAlarm(); });
            CloseAlarmCommand = new RelayCommand(() => { CloseAlarm(); });
            OpenBackgroundsCommand = new RelayCommand(() => { OpenBackgroundSelection(); });
            CloseBackgroundsCommand = new RelayCommand(() => { CloseBackgroundSelection(); });
            CloseApplicationCommand = new RelayCommand(CloseApplication);
        }

        private void ToggleDayMode()
        {
            IsSettingsVisible = false;
            IsDayMode = !IsDayMode;
            UpdateActiveUserControl();
        }

        private void OpenAlarm()
        {
            IsSettingsVisible = false;
            IsAlarmControlVisible = true;
        }

        private void CloseAlarm()
        {
            IsAlarmControlVisible = false;
        }

        private void OpenBackgroundSelection()
        {
            IsSettingsVisible = false;
            IsBackgroundSelectionVisible = true;
        }

        private void CloseBackgroundSelection()
        {
            IsBackgroundSelectionVisible = false;
        }

        private void CloseApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void UpdateActiveUserControl()
        {
            _brightnessService.SetBrightness(IsDayMode ? 1 : 0);

            if (_spotifyService != null && _spotifyService.IsPlaying)
            {
                ActiveUserControl = _musicView;
            }
            else if (IsDayMode)
            {
                ActiveUserControl = _dayView;
            }
            else
            {
                ActiveUserControl = _nightView;
            }
        }

        private bool HandlePropertyChange<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (value.Equals(field)) return false;
            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}