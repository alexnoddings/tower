using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Tower.Core.Services;

namespace Tower.Services.BingBackground
{
    public class BingBackgroundService : IBackgroundService, INotifyPropertyChanged
    {
        private static readonly TimeSpan NewBackgroundCheckInterval = TimeSpan.FromSeconds(15);

        private DateTime _lastBackgroundCheckDate = DateTime.MinValue;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _selectedBackgroundUri;
        public string SelectedBackgroundUri
        {
            get => _selectedBackgroundUri;
            private set
            {
                if (value == _selectedBackgroundUri) return;
                _selectedBackgroundUri = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _availableBackgroundUris;
        public List<string> AvailableBackgroundUris
        {
            get => _availableBackgroundUris;
            private set
            {
                if (value == _availableBackgroundUris) return;
                _availableBackgroundUris = value;
                NotifyPropertyChanged();
            }
        }

        public BingBackgroundService()
        {
            SelectedBackgroundUri = "";
            AvailableBackgroundUris = new List<string>();

            var timer = new DispatcherTimer(DispatcherPriority.Background) {Interval = NewBackgroundCheckInterval};
            timer.Tick += async (_, __) => await CheckForNewBackgroundsAsync();
            timer.Start();

            // Begin checking in the background
            Task.Factory.StartNew(async () => await CheckForNewBackgroundsAsync());
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async Task CheckForNewBackgroundsAsync()
        {
            if (DateTime.Today <= _lastBackgroundCheckDate) return;

            _lastBackgroundCheckDate = DateTime.Today;
            AvailableBackgroundUris = await BingHelper.GetImageUrisAsync();
            SelectedBackgroundUri = AvailableBackgroundUris.First();
        }
    }
}
