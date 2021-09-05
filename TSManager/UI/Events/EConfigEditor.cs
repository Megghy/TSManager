using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TSManager.Modules;

namespace TSManager.UI.Events
{
    internal class EConfigEditor : GUIEvents.GUIEventBase
    {
        public override string ControlPrefix => "CongifEditor";
        public override void OnButtonClick(Button sender)
        {
            switch (sender.Name)
            {
                case "ConfigEditor_Format":
                    ConfigEdit.Format(sender.DataContext.ToType<Data.ConfigData>());
                    break;
                case "ConfigEditor_Refresh":
                    ConfigEdit.LoadAllConfig();
                    break;
                case "ConfigEditor_Save":
                    ConfigEdit.Save(sender.DataContext.ToType<Data.ConfigData>());
                    break;
                case "ConfigEditor_OpenFile":
                    ConfigEdit.OpenFile(sender.DataContext.ToType<Data.ConfigData>());
                    break;
                default:
                    break;
            }
        }
    }
}
