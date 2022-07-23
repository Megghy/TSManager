using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace TSManager.Core.Modules
{
    public sealed class SimplePipeline : IDisposable
    {
        public string Id { get; init; }
        public readonly NamedPipeServerStream _pipeServer;

        public SimplePipeline() : this($"TSManager.Pipes.{Guid.NewGuid()}") { }
        public SimplePipeline(string pipeId)
        {
            Id = pipeId;
            _pipeServer = new NamedPipeServerStream(pipeId, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        }
        public async Task StartAsync()
        {
            await _pipeServer.WaitForConnectionAsync();
        }
        public void Dispose()
        {
            _pipeServer?.Close();
            _pipeServer?.Dispose();
        }
    }
}
