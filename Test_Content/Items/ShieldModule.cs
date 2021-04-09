using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Utils.Vector;
using Hopper.Utils.Chains;

namespace Hopper.Test_Content
{
    public class ShieldModule : TinkerModule
    {
        private IntVector2 m_relativeDir;
        private int m_pierceIncrease;
        private IItem m_item;

        private ShieldModule(IntVector2 relativeDir, int pierceIncrease) : base(null)
        {
            m_relativeDir = relativeDir;
            m_pierceIncrease = pierceIncrease;
            var builder = new ChainDefBuilder()
                .AddDef(Attackable.Check)
                .AddHandler(BlockDirection, PriorityRank.High)
                .AddDef(Attackable.Do)
                .AddHandler(AbsorbDamageAndBreak, PriorityRank.High)
                .End();
            m_tinker = new Tinker<TinkerData>(builder.ToStatic());
        }

        public override void Init(IItem item) => m_item = item;

        private IntVector2 GetRotatedRelativeDir(Entity actor)
        {
            var angle = actor.Orientation.AngleTo(IntVector2.Right);
            return m_relativeDir.Rotate(angle);
        }

        private void BlockDirection(Attackable.Event ev)
        {
            var blockDir = GetRotatedRelativeDir(ev.actor);
            if (ev.dir == -blockDir)
            {
                ev.resistance.pierce += m_pierceIncrease;
            }
        }

        private void AbsorbDamageAndBreak(Attackable.Event ev)
        {
            var blockDir = GetRotatedRelativeDir(ev.actor);
            if (ev.atkParams.attack.damage > 0 && ev.dir == -blockDir)
            {
                // TODO: dummy inventory that just calls the equip methods on item
                ev.actor.Inventory?.Destroy(m_item);
                ev.atkParams.attack.damage = 0;
            }
        }

        public static TinkerModule CreateFront(int pierceIncrease)
            => new ShieldModule(IntVector2.Right, pierceIncrease);
        public static TinkerModule CreateBack(int pierceIncrease)
            => new ShieldModule(IntVector2.Left, pierceIncrease);
        public static TinkerModule CreateRight(int pierceIncrease)
            => new ShieldModule(IntVector2.Down, pierceIncrease);
        public static TinkerModule CreateLeft(int pierceIncrease)
            => new ShieldModule(IntVector2.Up, pierceIncrease);
    }
}