using Chains;
using Hopper.Core.Behaviors;
using Hopper.Core.Stats;

namespace Hopper.Core
{
    public interface IStatus : IHaveId
    {
        void Update(Entity entity);
        bool IsApplied(Entity entity);
        // void Nullify(Entity entity);
        bool TryApplyAuto(Entity applicant, Entity target);
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

        public virtual void Update(Entity entity)
        {
            if (entity.Tinkers.IsTinked(m_tinker))
            {
                var data = m_tinker.GetStore(entity);
                UpdateAmount(data);

                if (data.amount <= 0)
                {
                    Remove(entity);
                }
            }
        }

        protected virtual void UpdateAmount(T store)
        {
            store.amount--;
        }

        public virtual bool IsApplied(Entity entity)
        {
            return m_tinker.IsTinked(entity) && m_tinker.GetStore(entity).amount > 0;
        }

        public bool TryApplyAuto(Entity applicant, Entity target)
            => TryApplyWithInitialData(applicant, target, new T());

        public bool TryApplyWithInitialData(Entity applicant, Entity target, T statusData)
            => TryApply(target, statusData, GetStat(applicant));

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
                Apply(target, statusData);
            }

            return success;
        }

        // By default, just update the amount
        protected virtual void Reapply(T existingData, T newData)
        {
            existingData.amount = newData.amount;
        }

        public StatusFile GetStat(Entity entity)
        {
            return m_statPath.Path(entity.Stats);
        }

        protected virtual void Apply(Entity target, T statusData)
        {
            m_tinker.Tink(target, statusData);
        }

        public virtual void Remove(Entity entity)
        {
            m_tinker.Untink(entity);
        }
    }
}