using Chains;
using Core;
using Core.Stats;
using Newtonsoft.Json;
using Core.Utils;
using Core.Behaviors;
using System;

namespace Test
{
    public class BindData : StatusData
    {
        [JsonConverter(typeof(Core.IHaveIdConverter<Entity>))]
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