using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.History;
using Hopper.Core.Items;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
{
    public class BowTinkerData : TinkerData
    {
        public int numCharges = 0;
    }


    public class Bow
    {
        public static readonly Attack.Source ArrowSource = new Attack.Source();
        public static readonly ISlot<IItemContainer<IItem>> Slot = Hopper.Core.Items.BasicSlots.RangeWeapon;
        public static readonly UpdateCode ToggledChargingUpdate = new UpdateCode("toggled_charging");
        public static ModularItem DefaultBow = CreateBow();


        // use this function to get a module for your item
        public static TinkerModule CreateModule(INormalShooting shooting)
        {
            return new Bow(shooting).ShootingModule;
        }
        // TODO: either add more functions to generate more types of these
        // or make Bow's members all public.

        // public Tinker<BowTinkerData> Tinker => m_shootTinker;
        public readonly TinkerModule ShootingModule;

        private Tinker<BowTinkerData> m_shootTinker;
        private UndirectedAction m_chargeAction;
        private DirectedAction m_shootAction;
        private INormalShooting m_shooting;

        private Bow(INormalShooting shooting)
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
            m_shootAction = Action.CreateSimple(Shoot);
            ShootingModule = new TinkerModule(m_shootTinker);
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

        public static readonly Attack defaultArrowAttack =
            new Attack
            {
                sourceId = ArrowSource.Id,
                power = 1,
                pierce = 1,
                damage = 1
            };

        public static ModularItem CreateBow()
        {
            var defaultShooting = new AnonShooting(
                new TargetLayers { skip = Layer.WALL, targeted = Layer.REAL }, defaultArrowAttack, null, true
            );
            var module = CreateModule(defaultShooting);
            return new ModularItem(new ItemMetadata("Default_Bow"), Slot, module);
        }
    }
}