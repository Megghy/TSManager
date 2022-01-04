using System.Text.RegularExpressions;
using TSManager.Core.Properties;

namespace TSManager.Core
{
    public class Localization
    {
        static readonly Regex regex = new(@"{\d+}");
        public static string Get(string key, params object[] param)
        {
            return Resources.ResourceManager.GetString(key) is { } text
                    ? regex.IsMatch(text)
                    ? string.Format(text, param)
                    : text
                    : key;
        }
    }
}
