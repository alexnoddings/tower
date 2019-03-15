using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower.Core.Services
{
    public interface ITimeService
    {
        string TimeText { get; }
        string DateText { get; }
        DateTime NextAlarm { get; }
        void SetNextAlarm(TimeSpan timeOfDay);
        event EventHandler AlarmTriggered;
    }
}
