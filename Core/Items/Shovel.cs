using System.Collections.Generic;
using Core.Items;
using Core.Targeting;

namespace Core.Items.Shovel
{
    public interface IShovel
    {
        List<Target> GetTargets();
    }

}