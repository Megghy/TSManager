using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Modules
{
    public static class PipeServer
    {
        private static NamedPipeServerStream pipeServer;
        internal static async Task CreatePipeServer()
        {
            pipeServer = new("TSManager.NamedPipeServer", PipeDirection.InOut);
            await pipeServer.WaitForConnectionAsync();
        }
    }
}
