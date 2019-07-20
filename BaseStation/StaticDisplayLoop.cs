using System;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStation
{
    public sealed class StaticDisplayLoop : IDisplayLoop
    {
        public DisplayDriver Driver { get; }
        public DisplayFrame Frame { get; }

        public StaticDisplayLoop(DisplayDriver driver, DisplayFrame frame)
        {
            driver.ThrowIfNull(nameof(driver));
            frame.ThrowIfNull(nameof(frame));

            Driver = driver;
            Frame = frame;
        }

        public Task LoopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);

            Driver.ShowFrame(Frame);
            return Task.FromResult(true);
        }
    }
}