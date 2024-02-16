namespace Utilities.Collections
{
	public static class CollectionExts
	{
		private static Random rng = new Random();
		/// <summary>
		/// Shuffles a list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
		public static int MinOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			if (source == null || selector == null)
			{
				return default;
			}

			int value;
			using (IEnumerator<TSource> e = source.GetEnumerator())
			{
				if (!e.MoveNext())
				{
					return default;
				}

				value = selector(e.Current);
				while (e.MoveNext())
				{
					int x = selector(e.Current);
					if (x < value)
					{
						value = x;
					}
				}
			}

			return value;
		}
	}
}
