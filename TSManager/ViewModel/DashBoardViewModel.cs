using Stylet;
using System.ComponentModel;
using TSManager.Model;

namespace TSManager.ViewModel
{
    public class DashBoardViewModel : Screen, IHaveDisplayName
    {
        public DashBoardViewModel()
        {
            DisplayName = Module.Localization.Get("MainWindow.Tab.DashBoard");
            currentServer = new() { name = "test" };
        }
        public ServerContainer currentServer;
        #region 属性
        public BindingList<TerrariaApi.Server.TerrariaPlugin> PluginList => currentServer?.Plugins;
        public string CurrentServerName => currentServer?.name;
        #endregion
        public double CPUUsage = 65;
    }
}
