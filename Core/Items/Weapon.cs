using System.Collections.Generic;
using Core.Items;

namespace Core.Items.Weapon
{
    public interface IWeapon
    {
        public static int slot = 1;
        public List<Target> GetTargets();
    }

}