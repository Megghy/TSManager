using Stylet;
using System;
using System.Reflection;
using TSManager.Core.Plugin;
using TSManager.GUI.ViewModel;

namespace TSManager.GUI
{
    public class TSMGUIPlugin : TSMPluginBase
    {
        public override string Name => "TSManager.GUI";
        public override string Description => "TSManager 用户界面模块";
        public override string Author => "Megghy";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new(1, 0);

        [STAThread]
        public override void Initialize()
        {
            App app = new();
            app.InitializeComponent();
            app.Run();
        }
    }
}
