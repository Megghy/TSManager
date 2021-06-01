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
        }
        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(ButtonTextBox));
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ButtonTextBox));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ButtonTextBox));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
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
