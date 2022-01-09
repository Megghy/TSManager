using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Modules.Pipe
{
    public class PipelinePool
    {
        public const int DefaultPoolSize = 1024;
        /// <summary>
        /// 用于存储和管理管道的进程池
        /// </summary>
        private static ConcurrentDictionary<string, Pipeline> PipeServerPool { get; } = new();
        /// <summary>
        /// 创建一个新的管道
        /// </summary>
        /// <returns>管道</returns>
        public static Pipeline CreatePipeLine(string pipeID = null)
        {
            pipeID ??= $"TSManager.Pipe.{Guid.NewGuid()}";
            lock (PipeServerPool)
            {
                if (PipeServerPool.Count < DefaultPoolSize)
                {
                    var pipe = new Pipeline(pipeID);
                    pipe.Start();
                    PipeServerPool.TryAdd(pipe.ID, pipe);
                    Logger.Info($"创建管道 => {pipeID}");
                    return pipe;
                }
                else
                {
                    Logger.Info($"超过最大管道数量限制 => {DefaultPoolSize}");
                    return null;
                }
            }
        }
        /// <summary>
        /// 根据ID从管道池中释放一个管道
        ///  <summary>
        public static void DisposePipeLine(string pipeID)
        {
            lock (PipeServerPool)
            {
                Logger.Info($"开始关闭管道 => {pipeID}");
                if (PipeServerPool.TryRemove(pipeID, out Pipeline pipe))
                    pipe.Dispose();
                else
                    Logger.Warn($"未找到管道 => {pipeID}");
            }
        }
        /// <summary>
        /// (异步)创建一个新的管道进程
        ///  <summary>
        public static async void CreatePipeLineAsync() => await Task.Run(() => CreatePipeLine());
        /// <summary>
        /// (异步)根据ID从管道池中释放一个管道
        ///  <summary>
        public static async void DisposablePipeLineAsync(string id) => await Task.Run(() => { DisposePipeLine(id); });
    }
}
