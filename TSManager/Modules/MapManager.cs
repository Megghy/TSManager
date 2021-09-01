using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSManager.Data;

namespace TSManager.Modules
{
    internal static class MapManager
    {
        internal static void Init()
        {
            var maps = MapData.GetAllMaps();
            TSMMain.GUI.Console_MapBox.ItemsSource = maps;
            if (TSMMain.Settings.World != string.Empty)
            {
                var lastMap = maps.SingleOrDefault(m => m.Path == TSMMain.Settings.World);
                if (lastMap == null)
                {
                    Utils.Notice("未找到上次启动时所选地图, 请重新选择", HandyControl.Data.InfoType.Info);
                }
                else
                {
                    TSMMain.GUI.Console_MapBox.SelectedItem = lastMap;
                }
            }
        }
    }
}
