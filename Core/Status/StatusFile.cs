namespace Core
{
    // Statused -> status.Apply(entity, flavor)
    // -> entity.TinkAndSave(tinker)
    // +  tinkerData.flavor = flavor
    public class StatusFile : StatFile
    {
        public int amount;
        public int power;
    }
}