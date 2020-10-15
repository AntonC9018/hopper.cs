using Chains;
using Core;
using Newtonsoft.Json;
using Utils;

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
        public BindStatus(IChainDef[] chainDefs, string statusName, IChainDef[] spice, int defaultResValue = 1)
            : base(null, statusName, defaultResValue)
        {
            this.m_chainDefinition = spice == null ? chainDefs : chainDefs.Concat(spice);
        }

        public BindStatus(IChainDef[] chainDefs, string statusName, int defaultResValue = 1)
            : base(chainDefs, statusName, defaultResValue)
        {
        }
    }
}