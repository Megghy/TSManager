using System.Threading;
using WPFLocalizeExtension.Engine;

namespace TSManager.Core
{
    internal class Init
    {
        public static void StartInit()
        {
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            LocalizeDictionary.Instance.Culture = Thread.CurrentThread.CurrentCulture;
        }
    }
}
