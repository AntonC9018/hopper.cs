using Core;
using Newtonsoft.Json;

namespace Test
{
    public class BindFlavor : Flavor, IHaveSpice
    {
        [JsonConverter(typeof(Core.IHaveIdConverter<Entity>))]
        public Entity whoApplied;

        private ITinker spice;
        public ITinker Spice => spice;

        public BindFlavor(Entity whoApplied, ITinker spice)
        {
            this.whoApplied = whoApplied;
            this.spice = spice;
        }

    }
}