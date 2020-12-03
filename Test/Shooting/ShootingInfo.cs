using System.Collections.Generic;
using Core;
using Core.Utils.Vector;

namespace Test
{
    public class ShootingInfo
    {
        public List<Entity> attacked_targets;
        public IntVector2 last_checked_pos;

        public ShootingInfo()
        {
            this.attacked_targets = new List<Entity>();
        }
    }
}