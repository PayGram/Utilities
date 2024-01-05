using System.Collections.Concurrent;

namespace Utilities.Collections
{
	public class SemaphoreSlimThCount : SemaphoreSlim
	{
		public SemaphoreSlimThCount(int initialCount, int maxCount) : base(initialCount, maxCount)
		{
		}
		public int WaitingThreads { get; set; } = 0;

	}
	public class SemaphoreSlimLockProvider<T> : ILockProvider<T> where T : notnull
	{
		private static readonly ConcurrentDictionary<T, SemaphoreSlimThCount> _lockDictionary = new();
		object syncObj = new();

		public void Wait(T elementToLock)
		{
			SemaphoreSlimThCount? sem;
			lock (syncObj)
			{
				if (_lockDictionary.TryGetValue(elementToLock, out sem))
					sem.WaitingThreads++;
				else
				{
					sem = new SemaphoreSlimThCount(1, 1);
					_lockDictionary.TryAdd(elementToLock, sem);
				}
			}
			sem.Wait();
		}

		public async Task WaitAsync(T elementToLock)
		{
			SemaphoreSlimThCount? sem;
			lock (syncObj)
			{
				if (_lockDictionary.TryGetValue(elementToLock, out sem))
					sem.WaitingThreads++;
				else
				{
					sem = new SemaphoreSlimThCount(1, 1);
					_lockDictionary.TryAdd(elementToLock, sem);
				}
			}
			await sem.WaitAsync();
		}

		public bool Release(T elementToLock, bool remove = true)
		{
			SemaphoreSlimThCount? sem;
			lock (syncObj)
			{
				if (_lockDictionary.TryGetValue(elementToLock, out sem))
				{
					sem.WaitingThreads--;
					if (sem.WaitingThreads == 0)
						_lockDictionary.Remove(elementToLock, out _);
				}
				else
				{
					return false;
				}
			}
			sem.Release();
			return true;
		}
	}
}
