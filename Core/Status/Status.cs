using Chains;
using Core.Behaviors;
using Core.Stats;

namespace Core
{
    public interface IStatus : IHaveId
    {
        void Update(Entity entity);
        bool IsApplied(Entity entity);
        // void Nullify(Entity entity);
        bool TryApply(Entity applicant, Entity target);
    }

    public static class Status
    {
        public class Resistance : MapFile
        {
            public static readonly StatPath<Resistance> Path = new StatPath<Resistance>("status/res");
            protected override MapFile DefaultFile => Path.DefaultFile;
        }
    }

    public class Status<T> : IStatus where T : StatusData, new()
    {
        private IStatPath<StatusFile> m_statPath;
        protected Tinker<T> m_tinker;
        public int Id => m_tinker.Id;
        public Tinker<T> Tinker => m_tinker;

        public Status(IChainDef[] chainDefs, IStatPath<StatusFile> statPath, int defaultResValue)
        {
            m_statPath = statPath;
            m_tinker = new Tinker<T>(chainDefs);
            Status.Resistance.Path.DefaultFile.Add(Id, defaultResValue);
        }

        // Returns false if it is still applied after the update 
        // Returns true if the status should be removed
        public virtual void Update(Entity entity)
        {
            if (entity.Tinkers.IsTinked(m_tinker))
            {
                var data = m_tinker.GetStore(entity);
                DecrementAmount(data);

                if (data.amount == 0)
                {
                    m_tinker.Untink(entity);
                }
            }
        }

        protected virtual void DecrementAmount(T store)
        {
            store.amount--;
        }

        public virtual bool IsApplied(Entity entity)
        {
            return m_tinker.IsTinked(entity) && m_tinker.GetStore(entity).amount > 0;
        }

        public bool TryApply(Entity applicant, Entity target)
            => TryApply(applicant, target, new T());

        public bool TryApply(Entity applicant, Entity target, T statusData)
            => TryApply(target, statusData, GetStat(applicant));

        // By default, just update the amount
        protected virtual void Reapply(T existingData, T newData)
        {
            existingData.amount = newData.amount;
        }

        // A convenience method for calling the Statused decorator
        public bool TryApply(Entity target, T statusData, StatusFile stat)
        {
            if (target.Behaviors.Has<Statused>() == false)
            {
                return false;
            }
            if (m_tinker.IsTinked(target))
            {
                Reapply(m_tinker.GetStore(target), statusData);
                return true;
            }

            statusData.amount = stat.amount;
            var pars = new Statused.Params(new StatusParam(this, stat));
            bool success = target.Behaviors.Get<Statused>().Activate(pars);

            if (success)
            {
                m_tinker.Tink(target, statusData);
            }

            return success;
        }

        public StatusFile GetStat(Entity entity)
        {
            return m_statPath.Path(entity.Stats);
        }
    }
}