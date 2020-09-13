using System.Collections.Generic;
using Core.Items;

namespace Core.Items.Weapon
{
    public interface IWeapon
    {
        public List<Target> GetTargets();
    }

}