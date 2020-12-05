using System.Collections.Generic;
using Hopper.Core;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
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