using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Tower.Core.Services;

namespace Tower.Application.ViewModel
{
    public class DayViewModel : ViewModelBase
    {
        public IBackgroundService BackgroundService { get; }

        public DayViewModel(IBackgroundService backgroundService)
        {
            BackgroundService = backgroundService;
        }
    }
}
