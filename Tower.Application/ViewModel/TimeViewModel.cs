using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Tower.Core.Services;

namespace Tower.Application.ViewModel
{
    public class TimeViewModel : ViewModelBase
    {
        public ITimeService TimeService { get; }

        public TimeViewModel(ITimeService timeService)
        {
            TimeService = timeService;
        }
    }
}
