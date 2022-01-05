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
        public Guid SID { get; init; } = Guid.Empty;
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string FilePath { get; private set; }
        public string FileDirectory => Directory.GetParent(FilePath).FullName;
        public override string ToString()
        {
            return $"{SID}_{Name}";
        }
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj?.ToString() == ToString();
        }

        public static bool operator ==(ServerInfo left, ServerInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ServerInfo left, ServerInfo right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return SID.GetHashCode();
        }
    }
}
