using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.Services
{
    public interface IBackgroundTaskQueue<T> where T: class
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
