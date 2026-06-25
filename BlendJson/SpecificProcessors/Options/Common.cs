using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BlendJson.SpecificProcessors.Options
{
	public static class Common
	{
		public static T[] ParseOptions<T>(JsonNode token, JsonSerializerOptions serializer) where T : new()
		{
			if (token is JsonObject mergeJObject)
			{
				return mergeJObject.Deserialize<T>(serializer).WrapToArray();
			}

            var valueKind = token.GetValueKind();

			if (valueKind == JsonValueKind.String)
			{
				var result = new T();
				var strOption = result as IStringOption;
				if (strOption == null)
					throw new SettingsException($"Wrong value: {token}. Expected string.");
				strOption.FillFromString(token.GetValue<string>());
				return result.WrapToArray();
			}

			if (token is JsonArray jArray)
			{
				var arr = jArray.Select(q =>
				{
                    var valueKind = q.GetValueKind();
                    var result = new T();
					if (valueKind == JsonValueKind.String && result is IStringOption strOption)
					{
						strOption.FillFromString(q.GetValue<string>());
						return result;
					}

					if (valueKind == JsonValueKind.Object)
					{
						return q.Deserialize<T>();
					}

					throw new Exception($"Wrong value: {q.GetPath()}");
				}).ToArray();
				return arr;
			}

			throw new SettingsException($"Cannot parse options: {token}");
		}
	}
}
