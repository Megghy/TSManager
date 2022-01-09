using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Models
{
    public struct TextInfo
    {
        public string Text { get; set; } = null;
        public Color ForegroundColor { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.Black;
    }
}
