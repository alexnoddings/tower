using System;
using System.Windows.Threading;
using Tower.Core.Services;

namespace Tower.Services.Time
{
    public class TimeService : BindableService, ITimeService
    {
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(1000);

        #region ITimeService
        private string _timeText;
        public string TimeText
        {
            get => _timeText;
            private set => HandlePropertyChange(ref _timeText, value);
        }

        private string _dateText;
        public string DateText
        {
            get => _dateText;
            private set => HandlePropertyChange(ref _dateText, value);
        }

        private DateTime _nextAlarm;
        public DateTime NextAlarm
        {
            get => _nextAlarm;
            set
            {
                TimeSpan timeOfDay = value.TimeOfDay;
                DateTime newValue;
                if (timeOfDay <= DateTime.Now.TimeOfDay)
                    newValue = DateTime.Today + TimeSpan.FromDays(1) + timeOfDay;
                else
                    newValue = DateTime.Today + timeOfDay;
                HandlePropertyChange(ref _nextAlarm, newValue);
            }
        }

        public event EventHandler AlarmTriggered;
        #endregion

        public TimeService()
        {
            NextAlarm = DateTime.Today + TimeSpan.FromHours(10) + TimeSpan.FromMinutes(16);

            Update();
            var updateTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = UpdateInterval };
            updateTimer.Tick += (_, __) => Update();
            updateTimer.Start();
        }

        private void Update()
        {
            TimeText = DateTime.Now.ToString("hh:mm:ss tt");
            DateText = DateTime.Now.ToString("yyyy-MM-dd");

            if (NextAlarm > DateTime.Now) return;

            AlarmTriggered?.Invoke(this, EventArgs.Empty);
            
            // This is done on the off chance the alarm is more than a day late, where it would keep adding 1 day on and invoking AlarmTriggered
            NextAlarm = DateTime.Today + TimeSpan.FromDays(1) + NextAlarm.TimeOfDay;
        }
    }
}
