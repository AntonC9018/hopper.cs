using Chains;
using Core;
using Core.Behaviors;
using Core.History;
using Core.Items;
using Core.Stats.Basic;
using Core.Utils.Vector;

namespace Test
{
    public class BowTinkerData : TinkerData
    {
        public bool isCharged = false;
    }

    public static class Bow
    {
        public static SimpleAction ChargeAction = new SimpleAction(ToggleCharging);
        public static readonly UpdateCode ToggledChargingUpdate = new UpdateCode("toggled_charging");

        public static void ToggleCharging(Entity entity, Action action)
        {
            var store = ShootTinker.GetStore(entity);
            store.isCharged = !store.isCharged;
            entity.History.Add(entity, ToggledChargingUpdate);
        }

        public static void SetChargeHandler(Controllable.Event ev)
        {
            ev.action = ChargeAction;
        }

        public static SimpleAction ShootAction = new SimpleAction(Shoot);

        private static Layer SkipLayer = Layer.WALL;
        private static Layer TargetedLayer = Layer.REAL;
        private static bool StopOnFailedAttack = true;

        private static Attack.Source ArrowSource = new Attack.Source();
        private static Attack ArrowAttack = new Attack
        {
            sourceId = ArrowSource.Id,
            power = 1,
            pierce = 1,
            damage = 1
        };

        public static readonly UpdateCode ShootingUpdate = new UpdateCode("shooting");

        public static void Shoot(Entity entity, Action action)
        {
            ShootTinker.GetStore(entity).isCharged = false;

            entity.Reorient(action.direction);
            entity.History.Add(entity, ShootingUpdate);

            IntVector2 currentOffsetVec = action.direction;

            while (true)
            {
                var cell = entity.GetCellRelative(currentOffsetVec);

                // off the world or a block is on the way
                if (cell == null || cell.GetEntityFromLayer(SkipLayer) != null)
                {
                    return;
                }

                var target = cell.GetEntityFromLayer(TargetedLayer);
                if (target != null && target.Behaviors.Has<Attackable>())
                {
                    var success = target.Behaviors.Get<Attackable>().Activate(
                        action.direction, new Attackable.Params(ArrowAttack, entity));

                    if (StopOnFailedAttack && success == false)
                    {
                        return;
                    }
                }

                currentOffsetVec += action.direction;
            }
        }

        public static void SetShootAction(Controllable.Event ev)
        {
            var store = ShootTinker.GetStore(ev);
            if (store.isCharged)
            {
                ev.action = ShootAction.WithDir(ev.action.direction);
            }
        }

        public static Tinker<BowTinkerData> ShootTinker = new Tinker<BowTinkerData>(
            new ChainDefBuilder()
                .AddHandler_ToAllVectorInputs(SetShootAction)
                .AddDef(Controllable.Chains[InputMapping.Weapon_1])
                .AddHandler(SetChargeHandler)
                .End().ToStatic()
        );

        // TODO: generate these
        public static int SlotId = 4;

        public static ModularItem Item =
            new ModularItem(SlotId, new TinkerModule(ShootTinker));
    }
}