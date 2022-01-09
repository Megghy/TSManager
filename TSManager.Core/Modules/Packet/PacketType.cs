using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Modules.Packet
{
    public enum PacketTypes
    {
        /// <summary>
        /// 服务器关闭请求
        /// </summary>
        Stop,

        /// <summary>
        /// 服务器基本信息
        /// </summary>
        ServerInfo,

        /// <summary>
        /// 某玩家的信息
        /// </summary>
        PlayerInfo,

        /// <summary>
        /// 所有玩家列表, 包括离线玩家
        /// </summary>
        PlayerList,

        /// <summary>
        /// 所有插件列表
        /// </summary>
        PluginsList,

        /// <summary>
        /// 玩家加入
        /// </summary>
        PlayerJoin,

        /// <summary>
        /// 玩家离开
        /// </summary>
        PlayerLeave,
    }
}
