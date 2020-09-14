using System.Collections.Generic;
using Core.Items;
using Core.Targeting;

namespace Core.Items.Weapon
{
    public interface IWeapon
    {
        List<Target> GetTargets();
    }

}