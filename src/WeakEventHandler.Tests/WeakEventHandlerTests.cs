﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace WeakEventHandler.Tests
{
    [TestFixture]
    public class WeakEventHandlerTests
    {
        public class Alarm
        {
            public event PropertyChangedEventHandler Beeped;
            public void Beep()
            {
                var handler = Beeped;
                if (handler is object)
                {
                    handler(this, new PropertyChangedEventArgs("Beep"));
                }
            }
        }

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

        [Test]
        public void ShouldHandleEventWhenBothReferencesAreAlive()
        {
            var alarm = new Alarm();
            var sleepy = new Sleepy(alarm);
            alarm.Beep();
            alarm.Beep();

            Assert.AreEqual(2, sleepy.SnoozeCount);
        }
    }
}
