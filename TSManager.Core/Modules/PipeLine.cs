using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace TSManager.Core.Modules
{
    public class Pipeline : IDisposable
    {
        public string ID { get; init; }
        private NamedPipeServerStream pipeServer;
        private Task pipeTask;
        private Task recieveOutputTask;
        private AutoResetEvent getInput, gotInput;
        private string inputContext;
        private StreamWriter pipeWriter;
        private StreamReader pipeReader;
        private bool shouldStop = false;
        public const int timeOutMillisecs = 10000; //10s

        public event RecieveMessageEventHandler RecieveMessage;
        public delegate void RecieveMessageEventHandler(string message);

        public Pipeline() : this($"TSManager.Pipes.{Guid.NewGuid()}") { }
        public Pipeline(string pipeID)
        {
            ID = pipeID;
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            pipeServer = new NamedPipeServerStream(pipeID, PipeDirection.InOut);
            recieveOutputTask = new(ReadLine);
        }
        public void Start()
        {
            pipeTask = Task.Run(PipeTask);
        }
        private void PipeTask()
        {
            pipeServer.WaitForConnection();
            recieveOutputTask.Start();
            try
            {
                pipeWriter = new StreamWriter(pipeServer)
                {
                    AutoFlush = true,
                };
                pipeReader = new StreamReader(pipeServer);
                while (true)
                {
                    RecieveMessage.Invoke(TryReadLine());
                }
            }
            catch (TimeoutException)
            {
                Logger.Error($"管道 {ID} 超时次数过多，视为丢失链接");
            }
            Logger.Info($"关闭管道 => {ID}");
            Dispose();
        }
        /// <summary>
        /// 向管道客户端发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(string msg)
        {
            pipeWriter.WriteLine(msg);
        }
        private void ReadLine()
        {
            while (!shouldStop)
            {
                getInput.WaitOne();
                inputContext = pipeReader.ReadLine();
                gotInput.Set();
            }
        }
        private string TryReadLine()
        {
            getInput.Set();
            if (gotInput.WaitOne(timeOutMillisecs))
                return inputContext;
            else
                return null;
        }
        public void Dispose()
        {
            pipeServer.Close();
            pipeServer.Dispose();
            getInput.Dispose();
            gotInput.Dispose();
        }
    }
}
