using System.Collections.Generic;
using Tower.Core.Services;

namespace Tower.Application.DesignTimeServices
{
    internal class DesignTimeBackgroundService : BindableService, IBackgroundService
    {
        private const string BackgroundUri =
            @"https://www.bing.com/az/hprichbg/rb/VinicuncaMountain_EN-GB0411988158_1920x1080.jpg";

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

        public DesignTimeBackgroundService()
        {
            AvailableBackgroundUris = new List<string> { BackgroundUri, BackgroundUri, BackgroundUri, BackgroundUri, BackgroundUri, BackgroundUri, BackgroundUri, BackgroundUri };
            SelectedBackgroundUri = BackgroundUri;
        }
    }
}
