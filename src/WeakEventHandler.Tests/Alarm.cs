using System.ComponentModel;

namespace WeakEventHandler.Tests
{
    public partial class WeakEventHandlerTests
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
    }
}
