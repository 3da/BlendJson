namespace BlendJson
{
	public static class CommonExtensions
	{
		public static T[] WrapToArray<T>(this T obj)
		{
			return new T[] { obj };
		}
	}
}
