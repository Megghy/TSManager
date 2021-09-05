using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using TSManager.UI.Control;

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
        }
        internal static readonly List<GUIEventBase> EventProcesserList = new();
        internal static void RegisterAll()
        {
#if DEBUG   
            if (!EventProcesserList.Any())
                EventProcesserList.Add(new Events.EConsole());
#endif

            EventProcesserList.ForEach(p => p.RegisteToBase(p.GetType()));
        }
        public static void OnGUIEvent(object sender, EventType type)
        {
            var name = sender?.GetType().GetProperty("Name")?.GetValue(sender) as string;
            EventProcesserList.FirstOrDefault(e => name.StartsWith(e.ControlPrefix))?.OnEvent(type, sender, name);
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
            public virtual void OnEvent(EventType type, object sender, string name)
            {
                if (EventList.TryGetValue(type, out var method))
                    method.Invoke(this, new object[] { sender});
            }
            #region 各种事件
            [GUIEvent(EventType.ButtonClick)]
            public virtual void OnButtonClick(Button sender) { }
            [GUIEvent(EventType.ButtonTextBoxClick)]
            public virtual void OnButtonTextBoxClick(ButtonTextBox sender) { }
            [GUIEvent(EventType.SwitchClick)]
            public virtual void OnSwichClick(ToggleButton sender) { }
            [GUIEvent(EventType.KeyDown)]
            public virtual void OnKeyDown(object sender) { }
            [GUIEvent(EventType.ComboSelectChange)]
            public virtual void OnComboSelectChange(ComboBox sender) { }
            [GUIEvent(EventType.ListViewSelect)]
            public virtual void OnListViewSelect(ButtonTextBox sender) { }
            [GUIEvent(EventType.ListBoxSelect)]
            public virtual void OnListBoxSelect(ButtonTextBox sender) { }

            #endregion
        }
    }
}
