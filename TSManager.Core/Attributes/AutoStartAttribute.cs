using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoStartAttribute : Attribute
    {
        public string PreMessage { get; set; }
        public string PostMessage { get; set; }
        public int Order { get; set; } = 100;
    }
}
