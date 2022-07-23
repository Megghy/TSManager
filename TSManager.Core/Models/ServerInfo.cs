using System;
using System.Diagnostics.CodeAnalysis;

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
            SID = Guid.NewGuid();
            Name = name;
            Description = des;
            FilePath = path;
        }
        /// <summary>
        /// 服务器唯一标识符
        /// </summary>
        public Guid SID { get; init; } = Guid.Empty;

        /// <summary>
        /// 服务器名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 服务器描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 启动文件路径
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// 服务器目录
        /// </summary>
        public string FileDirectory => Directory.GetParent(FilePath).FullName;
        public override string ToString()
            => $"{SID}_{Name}";
        public override bool Equals([NotNullWhen(true)] object? obj)
            => obj?.ToString() == ToString();

        public static bool operator ==(ServerInfo left, ServerInfo right)
            => left.Equals(right);

        public static bool operator !=(ServerInfo left, ServerInfo right)
            => !(left == right);

        public override int GetHashCode()
            =>SID.GetHashCode();
    }
}
