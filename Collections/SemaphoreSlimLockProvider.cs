using System.Collections.Concurrent;

namespace Utilities.Collections
{
    public class SemaphoreSlimLockProvider<T> : ILockProvider<T> where T : notnull
    {
        private static readonly ConcurrentDictionary<T, SemaphoreSlim> _lockDictionary = new();

        public void Wait(T elementToLock)
        {
            _lockDictionary.GetOrAdd(
                elementToLock, new SemaphoreSlim(1, 1)).Wait();
        }

        public async Task WaitAsync(T elementToLock)
        {
            await _lockDictionary.GetOrAdd(
                elementToLock, new SemaphoreSlim(1, 1)).WaitAsync();
        }

        public bool Release(T elementToLock, bool remove = true)
        {
            if (_lockDictionary.TryGetValue(elementToLock, out var semaphore))
            {
                semaphore.Release();
                if (remove)
                    return _lockDictionary.TryRemove(elementToLock, out _);
                return true;
            }
            else
                return false;
        }
    }
}
