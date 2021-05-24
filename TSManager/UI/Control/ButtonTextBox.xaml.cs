using System.Windows;
using System.Windows.Controls;
using PropertyChanged;

namespace TSManager.UI.Control
{
    /// <summary>
    /// ButtonTextBox.xaml 的交互逻辑
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class ButtonTextBox : UserControl
    {
        public ButtonTextBox()
        {
            InitializeComponent();
            DataContext = this;
        }
        public string ButtonText { get; set; } = "确认";
        public string Title { get; set; }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ButtonTextBox));
        public string Text
        {
            get { return ContentBox.Text; }
            set { SetValue(TextProperty, value); ContentBox.Text = value; }
        }
        /*public static readonly RoutedEvent deleteEvent = EventManager.RegisterRoutedEvent("ButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ButtonTextBox));
        public event RoutedEventHandler ButtonClick
        {
            add
            {
                AddHandler(deleteEvent, value);
            }

            remove
            {
                RemoveHandler(deleteEvent, value);
            }
        }*/
        public event RoutedEventHandler ButtonClick;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonClick(this, e);
        }
    }
}
