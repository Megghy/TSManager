using Stylet;
using System.ComponentModel;
using TSManager.Core.Servers;

namespace TSManager.GUI.ViewModel
{
    public class DashBoardViewModel : Screen, IHaveDisplayName
    {
        public DashBoardViewModel()
        {
            DisplayName = Core.Localization.Get("MainWindow.Tab.DashBoard");
            currentServer = new("test", "", null);
        }
        public ServerContainer currentServer;
        #region 属性
        public string CurrentServerName => currentServer?.Name;
        #endregion
        public double CPUUsage = 65;
    }
}
