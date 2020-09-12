using Core.Items;

namespace Core
{
    public class Player : Entity
    {
        public override bool IsPlayer => true;
        public Player() : base()
        {
            Inventory = new Inventory(this);
        }
    }
}