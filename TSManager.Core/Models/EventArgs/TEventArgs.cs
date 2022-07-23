using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.EventArgs
{
    public abstract class TEventArgs
    {
        public bool Handled { get; set; } = false;
    }
}
