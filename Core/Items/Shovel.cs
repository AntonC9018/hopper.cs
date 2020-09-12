using System.Collections.Generic;
using Core.Items;

namespace Core.Items.Shovel
{
    public interface IShovel
    {
        public List<Target> GetTargets();
    }

}