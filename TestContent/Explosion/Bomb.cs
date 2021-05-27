using Hopper.Core;
using Hopper.Shared.Attributes;
using Hopper.TestContent.SimpleMobs;

namespace Hopper.TestContent
{
    [EntityType]
    public static class BombEntity
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            SimplePassiveRealBase.AddComponents(subject);
            WillExplodeEntityModifier.AddTo(subject, 3);
        }

        public static void InitComponents(Entity subject)
        {
            SimplePassiveRealBase.InitComponents(subject);
            subject.GetWillExplodeEntityModifier().Preset(subject);
        }

        public static void Retouch(Entity subject){}
    }
}