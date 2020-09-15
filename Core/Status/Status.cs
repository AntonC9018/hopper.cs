using System;
using Core.Behaviors;

namespace Core
{
    public class Status<T> : IStatus
        where T : TinkerData, IHaveFlavor, new()
    {

        public int Id => m_id;
        protected readonly int m_id;
        private Tinker<T> m_tinker;

        public Status(Tinker<T> tinker)
        {
            m_tinker = tinker;
            m_id = IdMap.Status.Add(this);
        }

        public void Apply(Entity entity, Flavor f)
        {
            System.Console.WriteLine("Status applied");
            entity.TinkAndSave(m_tinker);
            var store = m_tinker.GetStore(entity);
            store.Flavor = f;

            if (f is IHaveSpice)
            {
                var tinker = ((IHaveSpice)f).Spice;
                entity.TinkAndSave(tinker);
            }
        }

        public void Tick(Entity entity)
        {
            if (!IsApplied(entity))
                return;
            var store = m_tinker.GetStore(entity);
            var f = store.Flavor;

            if (store != null && --f.amount <= 0)
            {
                entity.Untink(m_tinker);

                if (f is IHaveSpice)
                {
                    var tinker = ((IHaveSpice)f).Spice;
                    entity.Untink(tinker);
                }
            }
        }

        public bool IsApplied(Entity entity)
        {
            return entity.IsTinked(m_tinker);
        }
    }
}