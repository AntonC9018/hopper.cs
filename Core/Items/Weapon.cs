using System.Collections.Generic;
using Core.Items;

namespace Core.Items.Weapon
{
    public interface IWeapon
    {
        List<Target> GetTargets();
    }

}