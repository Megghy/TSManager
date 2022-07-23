using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace TSManager
{
    public static class Utils
    {
        public static readonly JsonSerializerOptions jsonOption = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
        public static T? DeserializeJson<T>(this string text)
        {
            if (text == "{}")
                return default;
            try
            {
                return JsonSerializer.Deserialize<T>(text, jsonOption);
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex}\r\n{text}");
                return default;
            }
        }
        public static object? DeserializeJson(this string text, Type type)
        {
            if (text == "{}")
                return default;
            try
            {
                return JsonSerializer.Deserialize(text, type, jsonOption);
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex}\r\n{text}");
                return default;
            }
        }
        public static string SerializeJson(this object o, JsonSerializerOptions option = null) 
            => JsonSerializer.Serialize(o, option ?? jsonOption);
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            foreach (T obj in source)
            {
                action(obj);
            }
        }
    }
}
