using System.Collections.Concurrent;
using Utilities.Collections;

public class SemaphoreSlimThCount : SemaphoreSlim
{
	public int MaxCount { get; private set; }
	public int WaitingThreads { get; set; } = 0;
	internal object Lock { get; private set; } = new object();
	public SemaphoreSlimThCount(int initialCount, int maxCount) : base(initialCount, maxCount)
	{
		MaxCount = maxCount;
	}
}
public class SemaphoreSlimLockProvider<T> : ILockProvider<T> where T : notnull
{
	private static readonly ConcurrentDictionary<T, SemaphoreSlimThCount> _lockDictionary = new();

	public void Wait(T elementToLock)
	{
		SemaphoreSlimThCount sem = _lockDictionary.GetOrAdd(elementToLock, new SemaphoreSlimThCount(1, 1));
		lock (sem.Lock)
		{
			_lockDictionary.TryAdd(elementToLock, sem); // this is needed because the element just gotten might have removed concurrently by the release statement
			sem.WaitingThreads++;
		}
		sem.Wait();
	}

	public async Task WaitAsync(T elementToLock)
	{
		SemaphoreSlimThCount sem = _lockDictionary.GetOrAdd(elementToLock, new SemaphoreSlimThCount(1, 1));
		lock (sem.Lock)
		{
			_lockDictionary.TryAdd(elementToLock, sem); // this is needed because the element just gotten might have removed concurrently by the release statement
			sem.WaitingThreads++;
		}
		await sem.WaitAsync();
	}

	public bool Release(T elementToLock, bool remove = true)
	{
		SemaphoreSlimThCount? sem = _lockDictionary.GetValueOrDefault(elementToLock);
		if (sem == null) return false;

		lock (sem.Lock)
		{
			sem.WaitingThreads--;
			sem.Release();
			if (remove || sem.WaitingThreads == 0)
			{
				_lockDictionary.Remove(elementToLock, out _);
			}
		}
		return true;
	}
}