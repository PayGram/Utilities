namespace Utilities.Collections
{
	public interface ILockProvider<T>
	{
		void Wait(T elementToLock);
		Task WaitAsync(T elementToLock);
		bool Release(T elementToLock, bool remove);
	}
}
