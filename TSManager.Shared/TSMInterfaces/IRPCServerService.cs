using System;
using System.Threading.Tasks;

namespace TSManager.Shared.TSMInterfaces
{
    public interface IRPCServerService
    {
        public Task NoticePlayerJoin();
    }
}
