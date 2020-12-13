namespace Hopper.Core.Retouchers
{
    public class CoreRetouchers
    {
        public Equip Equip = new Equip();
        public Invincibility Invincibility = new Invincibility();
        public Reorient Reorient = new Reorient();
        public Skip Skip = new Skip();

        public void RegisterAll(Registry registry)
        {
            Equip.RegisterAll(registry);
            Invincibility.RegisterAll(registry);
            Reorient.RegisterAll(registry);
            Skip.RegisterAll(registry);
        }
    }
}