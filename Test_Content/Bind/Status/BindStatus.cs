using Hopper.Core;
using Hopper.Core.Stat;

namespace Hopper.TestContent.Bind
{
    public class BindStatus : Status<BindData>
    {
        public BindStatus(IChainDef[] chainDefs, IStatPath<StatusFile> statusPath, int defaultResValue = 1)
            : base(chainDefs, statusPath, defaultResValue)
        {
        }
    }
}