namespace Core
{
    public class StatusParam
    {
        public Flavor flavor;
        public StatusFile statusStat;
        public int statusId;

        public StatusParam(Flavor flavor, StatusFile statusStat, int statusId)
        {
            this.flavor = flavor;
            this.statusStat = statusStat;
            this.statusId = statusId;
        }
    }
}