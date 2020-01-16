using NUnit.Framework;
using System;
using System.ComponentModel;

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

            Assert.That(sleepy.SnoozeCount, Is.EqualTo(2));
        }

        [Test]
        public void ShouldAllowSubscriberReferenceToBeCollectedByGC()
        {
            var alarm = new Alarm();
            var sleepyReference = null as WeakReference;
            new Action(() =>
            {
                // Run in other scope so that sleepy-variable will be
                // garbage collected
                var sleepy = new Sleepy(alarm);
                alarm.Beep();
                alarm.Beep();
                Assert.That(sleepy.SnoozeCount, Is.EqualTo(2));

                sleepyReference = new WeakReference(sleepy);
            })();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.That(sleepyReference.Target, Is.Null);
        }

        [Test]
        public void SubscriberShouldNotBeUnsubscribedUntilCollection()
        {
            var alarm = new Alarm();
            var sleepy = new Sleepy(alarm);
            var sleepyReference = new WeakReference(sleepy);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            alarm.Beep();
            alarm.Beep();

            Assert.That(sleepy.SnoozeCount, Is.EqualTo(2));

            // sleepy shall not be destroyed by GC, because we haven't left the scope yet, so
            // sleepy (strong reference is still active)
            Assert.That(sleepyReference.IsAlive, Is.True);
        }
    }
}
