using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.History;
using Hopper.Core.Items;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;
using System.Collections.Generic;
using System.Linq;

namespace Hopper.TestContent
{
    public class BowComponent : IComponent
    {

    }


    [EntityType]
    public static class Bow
    {
        public static Attack.Source ArrowSource = new Attack.Source();
        public static readonly ISlot<IItemContainer<IItem>> Slot = Hopper.Core.Items.BasicSlots.RangeWeapon;
        public static readonly UpdateCode ToggledChargingUpdate = new UpdateCode("toggled_charging");
        public static readonly Attack DefaultArrowAttack =
            new Attack
            {
                sourceId = ArrowSource.Id,
                power = 1,
                pierce = 1,
                damage = 1
            };
        public static readonly UnbufferedTargetProvider TargetProvider = 
            new UnbufferedTargetProvider(new StraightPattern(Layer.WALL), Layer.REAL);
            
        public static ModularItem DefaultItem = CreateDefault();

        public static ModularItem CreateDefault()
        {
            return new ModularItem(new ItemMetadata("Default_Bow"), Slot, CreateModule(DefaultShooting));
        }

        // use this function to get a module for your item
        public static TinkerModule CreateModule(INormalShooting shooting)
        {
            var bow = new Bow(shooting);
            return new TinkerModule(bow.m_shootTinker);
        }

        public readonly Tinker<BowTinkerData> m_shootTinker;
        public readonly UndirectedAction m_chargeAction;
        public readonly DirectedAction m_shootAction;
        public readonly INormalShooting m_shooting;

        public Bow(INormalShooting shooting)
        {
            m_shootTinker = new Tinker<BowTinkerData>(
                new ChainDefBuilder()
                    .AddHandler_ToAllVectorInputs(SetShootAction)
                    .AddDef(Controllable.Chains[InputMapping.Weapon_1])
                    .AddHandler(SetChargeHandler)
                    .End().ToStatic()
            );
            m_shooting = shooting;
            m_chargeAction = Action.CreateSimple(ToggleCharging);
            m_shootAction = Action.CreateSimple(Shoot, Predict);
        }

        private void Shoot(Entity entity, IntVector2 direction)
        {
            var store = m_shootTinker.GetStore(entity);
            if (store.numCharges > 0)
            {
                store.numCharges--;
                m_shooting.Shoot(entity, direction);
            }
        }

        private IEnumerable<IntVector2> Predict(Entity entity, IntVector2 direction)
        {
            if (m_shootTinker.GetStore(entity).numCharges > 0)
            {
                return m_shooting.Predict(entity, direction);
            }
            return Enumerable.Empty<IntVector2>();
        }

        private void ToggleCharging(Entity entity)
        {
            var store = m_shootTinker.GetStore(entity);
            store.numCharges = 1 - store.numCharges;
            entity.History.Add(entity, ToggledChargingUpdate);
        }

        private void SetChargeHandler(Controllable.Event ev)
        {
            ev.action = m_chargeAction;
        }

        private void SetShootAction(Controllable.Event ev)
        {
            var store = m_shootTinker.GetStore(ev);
            if (store.numCharges > 0)
            {
                ev.action = m_shootAction;
            }
        }
    }
}