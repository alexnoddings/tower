using System;
using System.Windows.Threading;
using Tower.Core.Services;

namespace Tower.Services.Time
{
    public class TimeService : BindableService, ITimeService
    {
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromSeconds(2);

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
            private set => HandlePropertyChange(ref _nextAlarm, value);
        }

        public void SetNextAlarm(TimeSpan timeOfDay)
        {
            if (timeOfDay <= DateTime.Now.TimeOfDay)
                NextAlarm = DateTime.Today + TimeSpan.FromDays(1) + timeOfDay;
            else
                NextAlarm = DateTime.Today + timeOfDay;
        }

        public event EventHandler AlarmTriggered;
        #endregion

        public TimeService()
        {
            SetNextAlarm(TimeSpan.FromHours(10) + TimeSpan.FromMinutes(16));

            Update();
            var updateTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = UpdateInterval };
            updateTimer.Tick += (_, __) => Update();
            updateTimer.Start();
        }

        private void Update()
        {
            TimeText = DateTime.Now.ToString("hh:mm:ss tt zz");
            DateText = DateTime.Now.ToString("yyyy-MM-dd");

            if (NextAlarm > DateTime.Now) return;

            AlarmTriggered?.Invoke(this, EventArgs.Empty);
            
            // This is done on the off chance the alarm is more than a day late, where it would keep adding 1 day on and invoking AlarmTriggered
            NextAlarm = DateTime.Today + TimeSpan.FromDays(1) + NextAlarm.TimeOfDay;
        }
    }
}
