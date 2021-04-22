using System.Collections.Generic;
using System.Runtime.Serialization;
using Hopper.Core.Components;
using Hopper.Utils;


namespace Hopper.Core.Items
{
    public partial class Inventory : IComponent
    {
        // This associates a slot it to an item id.
        // Note that the item id is NOT the entity (runtime) id, it is the TYPE id.
        // This means that any item can be picked up just once, unless it is countable.
        // The item itself is stored in the general storage.
        // Every slot can have at most 1 item.
        public Dictionary<Identifier, Identifier> _slots;

        // There are items that are stored independently, in a dictionary 
        // (that is, items without activation that can be collected idependently)
        // HasEquippable(), !HasItemActivation(), ?HasCountable()
        // Anyway, the item repartition is decentralized (items decide where to go themselves)
        // The id is the TYPE id, NOT the runtime id.
        public Dictionary<Identifier, Entity> _generalStorage;

        public List<Entity> _excess;

        // TODO: the slots should be injects?
        // TODO: a new attribute for copying the items on injection?
        // TODO: no, better add an initialization function for the entity type.
        public void Init()
        {
            _slots          = new Dictionary<Identifier, Identifier>();
            _generalStorage = new Dictionary<Identifier, Entity>();
            _excess         = new List<Entity>();
        }

        public bool MapSlot(Identifier slotId, out Identifier itemId)
        {
            return _slots.TryGetValue(slotId, out itemId);
        }

        public Entity GetItem(Identifier id)
        {
            return _generalStorage[id];
        }

        public Entity TryGetItem(Identifier id)
        {
            if (_generalStorage.ContainsKey(id))
                return _generalStorage[id];
            return null;
        }

        public bool TryGetItem(Identifier id, out Entity item)
        {
            return _generalStorage.TryGetValue(id, out item);
        }

        public void ReplaceForSlot(Identifier slotId, Entity item)
        {
            if (_slots.ContainsKey(slotId))
            {
                var otherItemId = _slots[slotId];
                _excess.Add(_generalStorage[otherItemId]);
                _generalStorage.Remove(otherItemId);
            }
            _slots[slotId] = item.typeId;
            _generalStorage[item.typeId] = item; 
        }

        public bool TryGetFromSlot(Identifier slotId, out Entity entity)
        {
            if (_slots.TryGetValue(slotId, out var itemId))
            {
                return _generalStorage.TryGetValue(itemId, out entity);
            }
            entity = null;
            return false;
        }

        public List<Entity> GetExcess() => _excess;
        public void ClearExcess() => _excess.Clear();

        public void Equip(Entity item)
        {
            Assert.That(!_generalStorage.ContainsKey(item.typeId));
            _generalStorage.Add(item.typeId, item);
        }

        public void IncreaseCount(Identifier itemId, int amount)
        {
            var otherCountable = _generalStorage[itemId].GetCountable();
            otherCountable.count += amount;
        }

        public Entity DropFromSlot(Identifier slotId)
        {
            return Drop(_slots[slotId]);
        }

        public Entity Drop(Identifier itemId)
        {
            var item = _generalStorage[itemId];
            _generalStorage.Remove(itemId);
            _excess.Add(item);
            return item;
        }

        // public void Replace(Entity item)
        // {
        //     if (_generalStorage.ContainsKey(item.typeId))
        //     {
        //         _excess.Add(_generalStorage[item.typeId]);
        //         _generalStorage.Remove(item.typeId);
        //     }
        //     _generalStorage.Add(
        // }
    }
}