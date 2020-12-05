using Hopper.Core.Behaviors;

namespace Hopper.Core
{
    public class StatusParam
    {
        public IStatus status;
        public StatusFile statusStat;

        public StatusParam(IStatus status, StatusFile statusStat)
        {
            this.status = status;
            this.statusStat = statusStat;
        }
    }
}