using Stylet;
using System.ComponentModel;
using TSManager.Core.Models;

namespace TSManager.GUI.ViewModel
{
    public class DashBoardViewModel : Screen, IHaveDisplayName
    {
        public DashBoardViewModel()
        {
            DisplayName = Core.Localization.Get("MainWindow.Tab.DashBoard");
            currentServer = new(new("test", ""));
        }
        public ServerContainer currentServer;
        #region 属性
        public string CurrentServerName => currentServer?.Info.Name ?? "unknown";
        #endregion
        public double CPUUsage = 65;
    }
}
