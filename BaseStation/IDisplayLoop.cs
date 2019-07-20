using System;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStation
{
    public interface IDisplayLoop
    {
        Task LoopAsync(CancellationToken cancelToken);
    }
}