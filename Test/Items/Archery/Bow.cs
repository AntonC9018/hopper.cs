using Chains;
using Core;
using Core.Behaviors;
using Core.History;
using Core.Items;
using Core.Stats.Basic;

namespace Test
{
    public class BowTinkerData : TinkerData
    {
        public int numCharges = 0;
    }


    public class Bow
    {
        // use this function to get a module for your item
        public static TinkerModule CreateModule(Shooting shooting)
        {
            return new Bow(shooting).ShootingModule;
        }
        // TODO: either add more functions to generate more types of these
        // or make Bow's members all public.


        // public Tinker<BowTinkerData> Tinker => m_shootTinker;
        public readonly TinkerModule ShootingModule;

        private Tinker<BowTinkerData> m_shootTinker;
        private SimpleAction m_chargeAction;
        private Action m_shootAction;
        private Shooting m_shooting;

        private Bow(Shooting shooting)
        {
            m_shootTinker = new Tinker<BowTinkerData>(
                new ChainDefBuilder()
                    .AddHandler_ToAllVectorInputs(SetShootAction)
                    .AddDef(Controllable.Chains[InputMapping.Weapon_1])
                    .AddHandler(SetChargeHandler)
                    .End().ToStatic()
            );
            m_shooting = shooting;
            m_chargeAction = new SimpleAction(ToggleCharging);
            m_shootAction = new SimpleAction(Shoot);
            ShootingModule = new TinkerModule(m_shootTinker);
        }

        private void Shoot(Entity entity, Action action)
        {
            var store = m_shootTinker.GetStore(entity);
            if (store.numCharges > 0)
            {
                store.numCharges--;
                m_shooting.Shoot(entity, action);
            }
        }

        private void ToggleCharging(Entity entity, Action action)
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
                ev.action = m_shootAction.WithDir(ev.action.direction);
            }
        }

        public static readonly ModularItem DefaultItem;
        public static readonly UpdateCode ToggledChargingUpdate = new UpdateCode("toggled_charging");
        public static readonly ISlot Slot = Core.Items.Slot.RangeWeapon;
        public static readonly Attack.Source ArrowSource = new Attack.Source();

        static Bow()
        {
            var defaultArrowAttack = new Attack
            {
                sourceId = ArrowSource.Id,
                power = 1,
                pierce = 1,
                damage = 1
            };
            var defaultShooting = new StaticShooting(
                Layer.REAL, Layer.WALL | Layer.DIRECTIONAL_WALL, defaultArrowAttack, null, true
            );
            var module = CreateModule(defaultShooting);
            DefaultItem = new ModularItem(Slot, module);
        }
    }
}