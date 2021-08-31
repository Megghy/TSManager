using System.Collections.Generic;
using Terraria;

namespace TSManager.Data
{
    public class MapData
    {
        public static List<MapData> GetAllMaps()
        {
            Main.LoadWorlds();
            var list = new List<MapData>();
            Main.WorldList.ForEach(w => list.Add(new MapData(w)));
            return list;
        }
        public MapData(Terraria.IO.WorldFileData data)
        {
            Name = data.Name;
            Path = data.Path;
        }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
