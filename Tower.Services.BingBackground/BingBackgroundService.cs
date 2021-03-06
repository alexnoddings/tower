﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Tower.Core.Services;

namespace Tower.Services.BingBackground
{
    public class BingBackgroundService : BindableService, IBackgroundService
    {
        private static readonly TimeSpan NewBackgroundCheckInterval = TimeSpan.FromSeconds(15);

        private DateTime _lastBackgroundCheckDate = DateTime.MinValue;

        #region IBackgroundService
        private string _selectedBackgroundUri;
        public string SelectedBackgroundUri
        {
            get => _selectedBackgroundUri;
            set => HandlePropertyChange(ref _selectedBackgroundUri, value);
        }

        private List<string> _availableBackgroundUris;
        public List<string> AvailableBackgroundUris
        {
            get => _availableBackgroundUris;
            private set => HandlePropertyChange(ref _availableBackgroundUris, value);
        }
        #endregion

        public BingBackgroundService()
        {
            SelectedBackgroundUri = "";
            AvailableBackgroundUris = new List<string>();

            // Begin checking in the background
            Task.Factory.StartNew(async () => await CheckForNewBackgroundsAsync());

            var timer = new DispatcherTimer(DispatcherPriority.Background) {Interval = NewBackgroundCheckInterval};
            timer.Tick += async (_, __) => await CheckForNewBackgroundsAsync();
            timer.Start();
        }

        private async Task CheckForNewBackgroundsAsync()
        {
            if (DateTime.Today <= _lastBackgroundCheckDate) return;

            _lastBackgroundCheckDate = DateTime.Today;
            AvailableBackgroundUris = await BingHelper.GetImageUrisAsync();
            SelectedBackgroundUri = AvailableBackgroundUris.First();
        }
    }
}
