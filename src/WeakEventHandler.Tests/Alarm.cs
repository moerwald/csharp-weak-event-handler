using System.ComponentModel;

namespace WeakEventHandler.Tests
{
    public partial class WeakEventHandlerTests
    {
        public class Alarm
        {
            public event PropertyChangedEventHandler Beeped;

            public void Beep() => Beeped?.Invoke(this, new PropertyChangedEventArgs("Beep"));
        }
    }
}
