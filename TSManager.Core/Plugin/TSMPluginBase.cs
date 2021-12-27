using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSManager.Core.Core.Servers;
using TSManager.Core.Models;

namespace TSManager.Core.Core.Plugin
{
    public abstract class TSMPluginBase 
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Author { get; }
        public abstract Version Version { get; }
        public abstract void Initialize();
        public virtual bool Dispose()
        {
            return false;
        }
    }
}
