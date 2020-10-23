using Chains;
using Core.Behaviors;
using Core.Stats;

namespace Core
{
    public interface IStatus : IHaveId
    {
        bool Update(Entity entity);
        bool IsApplied(Entity entity);
        void Nullify(Entity entity);
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

    public class Status<T> : Tinker<T>, IStatus where T : StatusData, new()
    {
        private IStatPath<StatusFile> m_statPath;

        public Status(IChainDef[] chainDefs, IStatPath<StatusFile> statPath, int defaultResValue) : base(chainDefs)
        {
            m_statPath = statPath;
            Status.Resistance.Path.DefaultFile.Add(m_id, defaultResValue);
        }

        // Returns true if it is still applied after the update 
        public virtual bool Update(Entity entity)
        {
            if (entity.Tinkers.IsTinked(this))
            {
                var data = GetStore(entity);
                data.amount--;
                return data.amount == 0;
            }
            return true;
        }

        public virtual bool IsApplied(Entity entity)
        {
            return entity.Tinkers.IsTinked(this) && GetStore(entity).amount > 0;
        }

        public virtual void Nullify(Entity entity)
        {
            Untink(entity);
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
            if (target.Tinkers.IsTinked(this))
            {
                Reapply(GetStore(target), statusData);
                return true;
            }

            statusData.amount = stat.amount;
            var pars = new Statused.Params(new StatusParam(this, stat));
            bool success = target.Behaviors.Get<Statused>().Activate(pars);

            if (success)
            {
                Tink(target, statusData);
            }

            return success;
        }

        public StatusFile GetStat(Entity entity)
        {
            return m_statPath.Path(entity.Stats);
        }
    }
}