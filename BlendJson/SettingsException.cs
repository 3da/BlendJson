using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlendJson
{
    public class SettingsException : Exception
    {
        public string? Path { get; set; }

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
            if (Path == null)
                return base.ToString();
            return "Error at path: " + Path + "\n" + base.ToString();
        }
    }
}
