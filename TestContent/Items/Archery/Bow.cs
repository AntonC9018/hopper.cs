using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;

namespace Hopper.TestContent
{
    public partial class BowComponent : IComponent
    {
        public bool _isCharged;

        public void ToggleCharge(Entity owner)
        {
            if (_isCharged)
            {
                // 
            }
            _isCharged = !_isCharged;
        }

        public static readonly UnbufferedTargetProvider TargetProvider = 
            new UnbufferedTargetProvider(new StraightPattern(Layer.WALL), Layer.REAL, Faction.Any);

        public static void Attack(Transform ownerTransform, IntVector2 direction)
        {
            // 1. get the attack (for now, get it from the owner
            ownerTransform.entity.GetStats().GetLazy(Core.Stat.Attack.Index, out var attack);
            // 2. get the targets
            foreach (var context in TargetProvider.GetTargets(ownerTransform.position, direction))
            {
                context.transform.entity.TryBeAttacked(null, attack, direction);
            }

            if (ownerTransform.entity.TryGetRangedWeapon(out var weapon))
            {
                weapon.GetBowComponent()._isCharged = false;
            }
        }

        public static DirectedAction AttackAction = Action.CreateSimple((actor, direction) => 
            Attack(actor.GetTransform(), direction));

        public static UndirectedAction RechargeAction = Action.CreateSimple(actor => 
            {
                if (actor.TryGetRangedWeapon(out var weapon))
                {
                    weapon.GetBowComponent().ToggleCharge(actor);
                }
            });
    }


    [EntityType]
    public static class Bow
    {
        [Slot("RangedWeapon")] public static Slot Slot = new Slot(true);
        public static Attack.Source ArrowSource = new Attack.Source();
        
        public static UndirectedAction GetChargeAction(Entity item, Entity owner) => BowComponent.RechargeAction;

        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            ItemBase.AddComponents(subject);
            Equippable.AddTo(subject, null);
            SlotComponent.AddTo(subject, Slot.Id);
            BowComponent.AddTo(subject);
            ItemActivation.AddTo(subject, GetChargeAction);
        }

        public static void InitComponents(Entity subject)
        {
            ItemBase.InitComponents(subject);
        }

        public static void Retouch(Entity subject)
        {
            ItemBase.Retouch(subject);
        }
    }
}