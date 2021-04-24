using Hopper.Shared.Attributes;

namespace Hopper.Core.Items
{
    [EntityType(Abstract = true)]
    public static class ItemBase
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Transform.AddTo(subject, Layer.ITEM);
            // History.History.AddTo(subject);

            // Item stuff
            // Equippable.AddTo(subject, new Identifier(0, 0));
            // Countable.AddTo(subject, 1);
            // ItemActivation.AddTo(subject);
        }

        public static void InitComponents(Entity subject)
        {
            // subject.GetEquippable().DefaultPreset();
            // subject.GetItemActivation().DefaultPreset();
        }

        public static void Retouch(Entity subject)
        {
            // Equippable.AddToInventoryUniqueHandlerWrapper.AddTo(subject);
            // Equippable.AddToInventoryCountableHandlerWrapper.AddTo(subject);
            // Equippable.AssignToInventorySlotCountableHandlerWrapper.AddTo(subject);
            // Equippable.AssignToInventorySlotUniqueHandlerWrapper.AddTo(subject);
        }
    }
}