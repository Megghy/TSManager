using System.Windows;
using System.Windows.Controls;

namespace TSManager.GUI.Control
{
    /// <summary>
    /// PerformanceBar.xaml 的交互逻辑
    /// </summary>
    public partial class PerformanceBar : UserControl
    {
        public PerformanceBar()
        {
            InitializeComponent();
        }
        public string Title { get; set; } = "null";
        public static readonly DependencyProperty PercentProperty = DependencyProperty.Register("Percent", typeof(int), typeof(PerformanceBar));
        public int Percent
        {
            get { return (int)(GetValue(PercentProperty) ?? 0); }
            set { SetValue(PercentProperty, value); }
        }
        public string DisplayPercent => $"{Percent} %";
    }
}
