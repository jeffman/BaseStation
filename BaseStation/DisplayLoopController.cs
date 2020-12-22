using System;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStation
{
    public class DisplayLoopController : IDisposable
    {
        private Task _loopTask;
        private CancellationTokenSource _cancelTokenSource;
        private int _guard;
        public bool IsRunning => _loopTask != null;

        public async Task StartNewLoop(IDisplayLoop loop)
        {
            if (Interlocked.Exchange(ref _guard, 1) == 1)
                throw new InvalidOperationException($"{nameof(StartNewLoop)} already in progress");

            try
            {
                await StopAndCompleteLoop();

                _loopTask = null;
                _cancelTokenSource = null;

                var cancelTokenSource = new CancellationTokenSource();
                var loopTask = loop.LoopAsync(cancelTokenSource.Token);

                if (loopTask == null)
                    throw new InvalidOperationException("The display loop task must not be null.");

                _loopTask = loopTask;
                _cancelTokenSource = cancelTokenSource;
            }
            finally
            {
                _guard = 0;
            }
        }

        public async Task StopAndCompleteLoop()
        {
            if (_loopTask == null)
                return;

            RequestCancelLoop();
            await _loopTask;
        }

        private void RequestCancelLoop()
        {
            _cancelTokenSource?.Cancel();
        }

        #region IDisposable

        private bool _isDisposed = false;

        ~DisplayLoopController()
        {
            System.Diagnostics.Debug.Assert(_isDisposed, "Dispose not called");
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_isDisposed)
                {
                    RequestCancelLoop();
                    _loopTask = null;
                    _cancelTokenSource = null;
                    _isDisposed = true;
                }
            }
        }

        #endregion
    }
}