using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Models
{
    /// <summary>
    /// 简易tr物品信息
    /// </summary>
    public struct TItem
    {
        public int Id { get; set; }
        public byte Prefix { get; set; }
        public int Stack { get; set; }
    }
}
