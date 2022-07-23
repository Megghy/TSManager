using TSManager.Shared.TSMInterfaces;

namespace TSManager.Core.Models
{
    public class DefaultRPCServerProcessor : IRPCServerService
    {
        public DefaultRPCServerProcessor(ServerContainer server)
        {
            _currentServer = server;
        }
        private ServerContainer _currentServer;
        public Task NoticePlayerJoin()
        {
            throw new System.NotImplementedException();
        }
    }
}
