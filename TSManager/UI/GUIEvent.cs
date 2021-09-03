using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace TSManager.UI
{
    internal class GUIEvent
    {
        public enum EventType
        {
            ButtonClick,

        }
        internal readonly static List<GUIEventBase> EventList = new();
        internal static void RegisterAll()
        {
            if (!EventList.Any())
                EventList.Add(new Events.ButtonClickEvent_Console());
            
        }
        public static void OnGUIEvent(object sender, EventType type)
        {
            var name = sender?.GetType().GetProperty("Name")?.GetValue(sender) as string;
            EventList.FirstOrDefault(e => name.StartsWith(e.ControlPrefix) && e.TargetEvent == type)?.OnEvent(sender, name);
        }
        public class GUIEventAttribute : Attribute
        {
        }
        public abstract class GUIEventBase
        {
            public abstract string ControlPrefix { get; }
            public abstract EventType TargetEvent { get; }
            public abstract void OnEvent(object sender, string name);
            public virtual string InternalName(string name) => name.Remove(0, ControlPrefix.Length + 1);
        }
    }
}
