using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Tower.Application.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isDayMode;
        public bool IsDayMode
        {
            get => _isDayMode;
            set => HandlePropertyChange(ref _isDayMode, value);
        }

        private bool _isSettingsVisible;
        public bool IsSettingsVisible
        {
            get => _isSettingsVisible;
            set => HandlePropertyChange(ref _isSettingsVisible, value);
        }

        public RelayCommand ToggleSettingsCommand { get; }
        public RelayCommand ToggleDayModeCommand { get; }
        public RelayCommand OpenAlarmsCommand { get; }
        public RelayCommand OpenBackgroundsCommand { get; }
        public RelayCommand CloseApplicationCommand { get; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                IsSettingsVisible = true;
            }
            else
            {
                IsSettingsVisible = false;
            }

            IsDayMode = true;

            ToggleSettingsCommand = new RelayCommand(() => IsSettingsVisible = !IsSettingsVisible);
            ToggleDayModeCommand = new RelayCommand(() => { IsSettingsVisible = false; ToggleDayMode(); });
            OpenAlarmsCommand = new RelayCommand(() => { IsSettingsVisible = false; OpenAlarms(); });
            OpenBackgroundsCommand = new RelayCommand(() => { IsSettingsVisible = false; OpenBackgrounds(); });
            CloseApplicationCommand = new RelayCommand(CloseApplication);
        }

        private void ToggleDayMode()
        {
            IsDayMode = !IsDayMode;
        }

        private void OpenAlarms()
        {

        }

        private void OpenBackgrounds()
        {

        }

        private void CloseApplication()
        {
            System.Windows.Application.Current.Shutdown();
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