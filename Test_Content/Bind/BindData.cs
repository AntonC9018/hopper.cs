using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Core.Stats;
using Newtonsoft.Json;

namespace Hopper.Test_Content
{
    public class BindData : StatusData
    {
        [JsonConverter(typeof(Hopper.Core.IHaveIdConverter<Entity>))]
        public Entity whoApplied;

        public BindData() { }

        public BindData(Entity whoApplied)
        {
            this.whoApplied = whoApplied;
        }
    }

    public class BindStatus : Status<BindData>
    {
        public BindStatus(IChainDef[] chainDefs, IStatPath<StatusFile> statusPath, int defaultResValue = 1)
            : base(chainDefs, statusPath, defaultResValue)
        {
        }
    }
}