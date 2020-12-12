using Hopper.Core;
using Newtonsoft.Json;

namespace Hopper.Test_Content.Bind
{
    public class BindData : StatusData
    {
        public Entity whoApplied;

        public BindData() { }

        public BindData(Entity whoApplied)
        {
            this.whoApplied = whoApplied;
        }
    }
}