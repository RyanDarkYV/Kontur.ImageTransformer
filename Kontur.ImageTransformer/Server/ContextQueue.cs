using System.Diagnostics;
using System.Net;

namespace Kontur.ImageTransformer.Server
{
    internal class ContextQueue
    {
        #region Properties
        public HttpListenerContext Context { get; private set; }
        private Stopwatch _timer;
        public int TimeInQueue => (int)_timer.Elapsed.TotalMilliseconds;
        #endregion

        #region Constructors
        public ContextQueue()
        {
            _timer = new Stopwatch();
        }

        public ContextQueue(HttpListenerContext context)
        {
            _timer = new Stopwatch();
            this.Context = context;
        }
        #endregion

        #region SetContext
        internal void SetContext(HttpListenerContext context)
        {
            Context = context;
        }
        #endregion

        #region Start/stop timer
        internal void StartTimer()
        {
            if (_timer != null && !_timer.IsRunning)
            {
                _timer.Start();
            }
        }

        internal void StopTimer()
        {
            if (_timer != null && _timer.IsRunning)
            {
                _timer.Stop();
            }
        }
        #endregion
    }
}