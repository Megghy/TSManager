using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.IO;

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
