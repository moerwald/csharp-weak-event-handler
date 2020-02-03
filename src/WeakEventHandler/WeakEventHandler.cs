using System;
using System.Reflection;

namespace WeakEventHandler
{
    public sealed class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly WeakReference _targetReference;
        private readonly MethodInfo _method;
        private readonly Action _targetIsGone;

        public WeakEventHandler(EventHandler<TEventArgs> callback)
        {
            _method = callback.Method;
            _targetReference = new WeakReference(callback.Target, true);
        }

        public WeakEventHandler(EventHandler<TEventArgs> callback, Action targetIsGone)
            : this(callback) 
            => _targetIsGone = targetIsGone;

        public void Handler(object sender, TEventArgs e)
        {
            var weakTargetReference = _targetReference.Target;
            if (GCDidntDestroy(weakTargetReference))
            {
                var callback = Delegate.CreateDelegate(
                                            typeof(EventHandler<TEventArgs>),
                                            weakTargetReference,
                                            _method,
                                            true
                                        ) as EventHandler<TEventArgs>;
                callback?.Invoke(sender, e);
            }
            else
            {
                _targetIsGone?.Invoke();
            }

            bool GCDidntDestroy(object t) => t is object;
        }
    }
}
