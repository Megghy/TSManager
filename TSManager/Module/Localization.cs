using System.Reflection;
using WPFLocalizeExtension.Extensions;

namespace TSManager.Module
{
    public class Localization
    {
        public static string Get(string key, object[] obj = null)
        {
            try
            {
                return LocExtension.GetLocalizedValue<string>(Assembly.GetCallingAssembly().GetName().Name + ":Resources:" + key);
            }
            catch
            {
                return key;
            }
        }
    }
}
