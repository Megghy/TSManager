using System.Windows;

namespace TSManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Core.Init.StartInit();
            base.OnStartup(e);
        }
    }
}
