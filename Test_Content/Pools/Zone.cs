using System.Collections.Generic;

namespace Hopper.Test_Content
{
    public abstract class PoolDef
    {
        public string path;
        public string Folder => path + '/';
        public string All => path + "/*";
        public string Current => path + "/~";
    }

    public class EntityPool
    {
        public List<Zone> Zones;

        public EntityPool(int numZones)
        {
            Zones = new List<Zone>(numZones);
            for (int i = 0; i < numZones; i++)
            {
                Zones.Add(new Zone($"zone{i}"));
            }
        }

        public static EntityPool Default = new EntityPool(2);
    }

    public class Zone : PoolDef
    {
        public Rarity Rare;
        public Rarity Uncommon;
        public Rarity Common;

        public Zone(string path)
        {
            this.path = path;
            Rare = new Rarity(path + "/rare");
            Uncommon = new Rarity(path + "/uncom");
            Common = new Rarity(path + "/com");
        }
    }

    public class Rarity : PoolDef
    {
        public Rarity(string path)
        {
            this.path = path;
        }
    }
}