using System.Windows;
using System.Windows.Controls;
using TSManager.Data;
using TSManager.Modules;

namespace TSManager.UI.Events
{
    internal class EDownload : GUIEvents.GUIEventBase
    {
        public override string ControlPrefix { get; } = "Download";
        public override void OnButtonClick(Button sender, RoutedEventArgs e)
        {
            switch (sender.Name)
            {
                case "Download_TShock_Refresh":
                    DownloadManager.RefreshAsync();
                    break;
                default:
                    break;
            }
        }
        public override void OnListViewSelectChange(ListView sender, SelectionChangedEventArgs e)
        {
            switch (sender.Name)
            {
                case "Download_TShock_List":
                    DownloadManager.SelectTSFile((DownloadInfo.TShockInfo)sender.SelectedItem);
                    break;
                default:
                    break;
            }
        }
    }
}
