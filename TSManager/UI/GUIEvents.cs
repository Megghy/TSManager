using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using HandyControl.Controls;
using TSManager.UI.Control;
using ComboBox = HandyControl.Controls.ComboBox;

namespace TSManager.UI
{
    internal class GUIEvents
    {
        public enum EventType
        {
            Class,
            ButtonClick,
            ButtonTextBoxClick,
            SwitchClick,
            KeyDown,
            ComboSelectChange,
            ListViewSelect,
            ListBoxSelect,
            DataGridSelectChange,
            TextInput,
            SelectedColorChange
        }
        internal static readonly List<GUIEventBase> EventProcesserList = new();
        internal static void RegisterAll()
        {
            Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "TSManager.UI.Events" && t.BaseType.Name != "Object").ForEach(t =>
            EventProcesserList.Add(Activator.CreateInstance(t) as GUIEventBase));
#if DEBUG   
            if (!EventProcesserList.Any())
                EventProcesserList.Add(new Events.EConsole());
#endif
            EventProcesserList.ForEach(p => p.RegisteToBase(p.GetType()));
        }
        public static void OnGUIEvent(object sender, object e, EventType type)
        {
            var name = sender?.GetType().GetProperty("Name")?.GetValue(sender) as string;
            EventProcesserList.FirstOrDefault(e => name.StartsWith(e.ControlPrefix))?.OnEvent(type, sender, e);
        }
        public class GUIEventAttribute : Attribute
        {
            public GUIEventAttribute(EventType e)
            {
                Event = e;
            }
            public EventType Event { get; set; }
        }
        public abstract class GUIEventBase
        {
            public abstract string ControlPrefix { get; }
            internal Dictionary<EventType, MethodInfo> EventList { get; set; } = new();
            internal void RegisteToBase(Type fromType)
            {
                var tempDict = new Dictionary<string, EventType>();
                GetType()
                    .GetMethods()
                    .Where(m => m.GetCustomAttribute<GUIEventAttribute>(true) is { })
                    .ForEach(m => tempDict.Add(m.Name, m.GetCustomAttribute<GUIEventAttribute>(true).Event));
                fromType
                    .GetMethods()
                    .Where(m => tempDict.ContainsKey(m.Name))
                    .ForEach(m => EventList.Add(tempDict[m.Name], m));
            }
            public virtual void OnEvent(EventType type, object sender, object e)
            {
                try
                {
                    if (EventList.TryGetValue(type, out var method))
                        method.Invoke(this, new object[] { sender, e });
                }
                catch (Exception ex) { ex.ShowError(); }
            }
            #region 各种事件
            [GUIEvent(EventType.ButtonClick)]
            public virtual void OnButtonClick(Button sender, RoutedEventArgs e) { }
            [GUIEvent(EventType.ButtonTextBoxClick)]
            public virtual void OnButtonTextBoxClick(ButtonTextBox sender, RoutedEventArgs e) { }
            [GUIEvent(EventType.SwitchClick)]
            public virtual void OnSwichClick(ToggleButton sender, RoutedEventArgs e) { }
            [GUIEvent(EventType.KeyDown)]
            public virtual void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e) { }
            [GUIEvent(EventType.ComboSelectChange)]
            public virtual void OnComboSelectChange(ComboBox sender, SelectionChangedEventArgs e) { }
            [GUIEvent(EventType.ListViewSelect)]
            public virtual void OnListViewSelectChange(ListView sender, SelectionChangedEventArgs e) { }
            [GUIEvent(EventType.ListBoxSelect)]
            public virtual void OnListBoxSelectChange(ListBox sender, SelectionChangedEventArgs e) { }
            [GUIEvent(EventType.DataGridSelectChange)]
            public virtual void OnDataGridSelectChange(DataGrid sender, SelectionChangedEventArgs e) { }
            [GUIEvent(EventType.TextInput)]
            public virtual void OnTextInput(object sender, EventArgs e) { }
            [GUIEvent(EventType.SelectedColorChange)]
            public virtual void OnSelectedColorChanged(ColorPicker sender, HandyControl.Data.FunctionEventArgs<System.Windows.Media.Color> e) { }

            #endregion
        }
    }
}
