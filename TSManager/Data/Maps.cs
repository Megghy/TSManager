﻿using System.Collections.Generic;
using Terraria;

namespace TSManager.Data
{
    public class Maps
    {
        public static List<Maps> GetAllMaps()
        {
            Main.LoadWorlds();
            var list = new List<Maps>();
            Main.WorldList.ForEach(w => list.Add(new Maps(w.Name, w.Path)));
            return list;
        }
        public Maps(string name, string path)
        {
            Name = name;
            Path = path;
        }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
