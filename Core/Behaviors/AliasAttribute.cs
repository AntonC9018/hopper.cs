using System;

namespace Hopper.Core.Behaviors
{
    public class AliasAttribute : Attribute
    {
        public AliasAttribute(params string[] aliases)
        {
        }
    }
}