using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.GUI.ViewModel
{
    public class ConsoleViewModel : Screen, IHaveDisplayName
    {
        public ConsoleViewModel()
        {
            DisplayName = Core.Localization.Get("MainWindow.Tab.DashBoard");
        }
    }
}
