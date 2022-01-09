using HandyControl.Data;
using PropertyChanged;
using Stylet;
using StyletIoC;
using System.Linq;
using TSManager.Core;

namespace TSManager.GUI.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class TSMViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly IContainer container;
        private readonly IWindowManager windowManager;

        public DrawerViewModel DrawerViewModel => container.Get<DrawerViewModel>();
        public TSMViewModel(IContainer container, IWindowManager windowManager, DashBoardViewModel dashBoard, ConsoleViewModel console)
        {
            this.container = container;
            this.windowManager = windowManager;

            Items.Add(console);
            Items.Add(dashBoard);

            ActiveItem = dashBoard;
        }

        public void NavigateTo(IScreen screen)
        {
            ActivateItem(screen);
        }
        public void CloseWindow()
        {
            RequestClose();
        }

        public void OnMenuSelectChange(object sender, FunctionEventArgs<object> args)
        {
            throw new();
        }
        public void OpenServerList() => IsServerListOpen = true;
        public bool IsServerListOpen { get; set; } = false;
    }
}
