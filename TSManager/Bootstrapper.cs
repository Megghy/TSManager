using TSManager.View;
using Stylet;
using StyletIoC;
using TSManager.ViewModel;

namespace TSManager
{
    public class Bootstrapper : Bootstrapper<TSMViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
        }
    }
}
