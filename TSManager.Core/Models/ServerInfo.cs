using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Models
{
    public struct ServerInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">服务器显示名称</param>
        /// <param name="path">服务器启动文件路径</param>
        /// <param name="des">描述 (可空</param>
        public ServerInfo(string name, string path, string des = null)
        {
            if (!File.Exists(path))
                throw new Exception($"Startup file not fount at: {path}");
            SID = Guid.NewGuid();
            Name = name;
            Description = des;
            FilePath = path;
        }
        public Guid SID { get; init; } = Guid.Empty;
        public string Name { get; private set; }    
        public string Description { get; private set; }
        public string FilePath { get; private set; }
        public string FileDirectory => Directory.GetParent(FilePath).FullName;
    }
}
