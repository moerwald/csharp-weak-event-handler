using System;
using System.Reflection;

namespace WeakEventHandler
{
    public sealed class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _method;

        public WeakEventHandler(EventHandler<TEventArgs> callback)
        {
            _method = callback.Method;
            _targetReference = new WeakReference(callback.Target, true);
        }

        public void Handler(object sender, TEventArgs e)
        {
            var target = _targetReference.Target;
            if (GCDidntDestroy(target))
            {
                var callback = (EventHandler<TEventArgs>)Delegate.CreateDelegate(
                    typeof(EventHandler<TEventArgs>),
                    target,
                    _method,
                    true);
                if (callback is object)
                {
                    callback(sender, e);
                }
            }

            bool GCDidntDestroy(object t) => t is object;
        }
    }
}
