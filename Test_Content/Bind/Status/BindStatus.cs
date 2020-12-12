using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Core.Stats;

namespace Hopper.Test_Content.Bind
{
    public class BindStatus : Status<BindData>
    {
        public BindStatus(IChainDef[] chainDefs, IStatPath<StatusFile> statusPath, int defaultResValue = 1)
            : base(chainDefs, statusPath, defaultResValue)
        {
        }
    }
}