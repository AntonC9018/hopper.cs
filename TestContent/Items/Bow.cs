using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.Items
{
    public partial class BowComponent : IComponent, IUndirectedActivateable, IStandartActivateable
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
            new UnbufferedTargetProvider(new StraightPattern(Layers.WALL), Layers.REAL, Faction.Any);

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

        bool IUndirectedActivateable.Activate(Entity entity)
        {
            if (entity.TryGetRangedWeapon(out var weapon))
            {
                weapon.GetBowComponent().ToggleCharge(entity);
                return true;
            }
            return false;
        }

        bool IStandartActivateable.Activate(Entity entity, IntVector2 direction)
        {
            Attack(entity.GetTransform(), direction);
            return true;
        }
    }


    [EntityType]
    public static class Bow
    {
        [Slot("RangedWeapon")] public static Slot Slot = new Slot(true);
        public static Attack.Source ArrowSource = new Attack.Source();
        
        public static UndirectedActivatingAction<BowComponent> GetChargeAction(Entity item, Entity owner) => BowComponent.UAction;

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