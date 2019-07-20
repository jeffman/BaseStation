using System;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStation
{
    public sealed class BusyDisplayLoop : IDisplayLoop
    {
        public DisplayDriver Driver { get; }
        public int Delay { get; }

        public BusyDisplayLoop(DisplayDriver driver, int delay)
        {
            driver.ThrowIfNull(nameof(driver));
            if (delay < 1)
                throw new ArgumentOutOfRangeException(nameof(delay));

            Driver = driver;
            Delay = delay;
        }

        public async Task LoopAsync(CancellationToken cancellationToken)
        {
            var iterator = new BusyFrameIterator();
            foreach (var frame in iterator)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                Driver.ShowFrame(frame);

                try
                {
                    await Task.Delay(Delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}