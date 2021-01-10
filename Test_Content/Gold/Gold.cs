using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
{
    public interface IGold
    {
        int Amount { get; set; }
    }

    public class Gold : Entity, IGold
    {
        public int Amount { get; set; }
        public override Layer Layer => Layer.GOLD;

        public static Gold Drop(IntVector2 pos, int amount, World world)
            => Drop(pos, amount, world, Factory);

        public static Gold Drop(IntVector2 pos, int amount, World world, IFactory<Gold> factory)
        {
            var cell = world.Grid.GetCellAt(pos);
            if (cell == null) return null;
            var gold = (Gold)cell.GetAnyEntityFromLayer(Layer.GOLD);
            if (gold == null)
            {
                gold = world.SpawnEntity(Factory, pos);
            }
            gold.Amount += amount;
            // TODO: save update of amount to history
            // gold.History.Add(UpdateCode.amount_changed)
            return gold;
        }

        public static EntityFactory<Gold> Factory = CreateFactory();
        public static readonly Retoucher PickUpRetoucher =
            Retoucher.SingleHandlered(Displaceable.Do, PickUp);

        public static EntityFactory<Gold> CreateFactory()
        {
            return new EntityFactory<Gold>()
                .AddBehavior(Attackable.DefaultPreset);
        }

        private static void PickUp(Displaceable.Event ev)
        {
            var golds = ev.actor.GetCell().GetAllFromLayer(Layer.GOLD);
            foreach (var gold in golds)
            {
                if (gold != null && gold.IsDead == false)
                {
                    var igold = (IGold)gold;
                    // ((ICanPickupGold)ev.actor).UpdateAmount(gold.m_amount);
                    System.Console.WriteLine($"Updated amount of gold by {igold.Amount}");
                    igold.Amount = 0;
                    gold.Die();
                }
            }
        }

    }
}