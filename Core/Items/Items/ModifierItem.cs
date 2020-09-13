namespace Core.Items
{
    public class ModifierItem : Item
    {
        Modifier modifier;

        public ModifierItem(Modifier modifier, int slot = 0)
        {
            this.modifier = modifier;
            this.slot = slot;
        }

        public override void BeDestroyed(Entity entity)
        {
            entity.StatManager.RemoveModifier(modifier);
        }

        public override void BeEquipped(Entity entity)
        {
            entity.StatManager.AddModifier(modifier);
        }

        public override void BeUnequipped(Entity entity)
        {
            entity.StatManager.RemoveModifier(modifier);
            entity.World.CreateDroppedItem(this, entity.Pos);
        }
    }
}