using System;
using System.Text.Json.Nodes;

namespace BlendJson
{
    public class SettingsException : Exception
    {
        public JsonNode? JToken { get; set; }

        public SettingsException()
        {
        }

        public SettingsException(string message) : base(message)
        {
        }

        public SettingsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public override string ToString()
        {
            if (JToken == null)
                return base.ToString();
            return JToken.GetPath() + "\n" + JToken + "\n" + base.ToString();
        }
    }
}
