using System.Drawing;

namespace TSManager.Shared.TSMDatastructs
{
    public struct ConsoleMessageInfo
    {
        public ConsoleMessageInfo()
        {
        }

        public string Text { get; set; } = string.Empty;
        public Color ForegroundColor { get; set; } = Color.Gray;
        public Color BackgroundColor { get; set; } = Color.Black;
    }
}
