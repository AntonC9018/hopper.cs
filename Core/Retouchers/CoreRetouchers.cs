namespace Hopper.Core.Retouchers
{
    public class CoreRetouchers
    {
        public AttacknessRetouchers Attackness = new AttacknessRetouchers();
        public Equip Equip = new Equip();
        public Invincibility Invincibility = new Invincibility();
        public Reorient Reorient = new Reorient();
        public Skip Skip = new Skip();

        public void RegisterAll(Registry registry)
        {
            Attackness.RegisterAll(registry);
            Equip.RegisterAll(registry);
            Invincibility.RegisterAll(registry);
            Reorient.RegisterAll(registry);
            Skip.RegisterAll(registry);
        }
    }
}