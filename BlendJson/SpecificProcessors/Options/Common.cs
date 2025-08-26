using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlendJson.SpecificProcessors.Options
{
	public static class Common
	{
		public static T[] ParseOptions<T>(JToken token, JsonSerializer serializer) where T : new()
		{
			if (token is JObject mergeJObject)
			{
				return mergeJObject.ToObject<T>(serializer).WrapToArray();
			}

			if (token.Type == JTokenType.String)
			{
				var result = new T();
				var strOption = result as IStringOption;
				if (strOption == null)
					throw new SettingsException($"Wrong value: {token}. Expected string.");
				strOption.FillFromString(token.ToObject<string>());
				return result.WrapToArray();
			}

			if (token is JArray jArray)
			{
				var arr = jArray.Children().Select(q =>
				{

					var result = new T();
					if (q.Type == JTokenType.String && result is IStringOption strOption)
					{
						strOption.FillFromString(q.ToObject<string>());
						return result;
					}

					if (q.Type == JTokenType.Object)
					{
						return q.ToObject<T>();
					}

					throw new Exception($"Wrong value: {q.Path}");
				}).ToArray();
				return arr;
			}

			throw new SettingsException($"Cannot parse options: {token}");
		}
	}
}
