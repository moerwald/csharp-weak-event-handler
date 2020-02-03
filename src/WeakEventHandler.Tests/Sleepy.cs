using System;
using System.ComponentModel;

namespace WeakEventHandler.Tests
{
    public partial class WeakEventHandlerTests
    {
        public class Sleepy
        {
            private readonly Alarm _alarm;
            private PropertyChangedEventHandler _handler;
            private int _snoozeCount;

            public Sleepy(Alarm alarm)
            {
                _alarm = alarm;
                _handler = new WeakEventHandler<PropertyChangedEventArgs>(
                                        Alarm_Beeped,
                                        () => _alarm.Beeped -= _handler
                                        ).Handler;
                _alarm.Beeped += _handler ;
            }

            private void Alarm_Beeped(object sender, PropertyChangedEventArgs e) => _snoozeCount++;

            public int SnoozeCount => _snoozeCount;
        }
    }
}
