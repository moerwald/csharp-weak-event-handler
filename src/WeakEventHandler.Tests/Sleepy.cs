using System.ComponentModel;

namespace WeakEventHandler.Tests
{
    public partial class WeakEventHandlerTests
    {
        public class Sleepy
        {
            private readonly Alarm _alarm;
            private int _snoozeCount;

            public Sleepy(Alarm alarm)
            {
                _alarm = alarm;
                _alarm.Beeped +=
                    new WeakEventHandler<PropertyChangedEventArgs>(Alarm_Beeped).Handler;
            }

            private void Alarm_Beeped(object sender, PropertyChangedEventArgs e)
            {
                _snoozeCount++;
            }

            public int SnoozeCount => _snoozeCount;
        }
    }
}
