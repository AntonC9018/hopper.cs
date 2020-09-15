namespace Core
{
    public class Flavor
    {
        public int amount;
    }

    public interface IHaveFlavor
    {
        Flavor Flavor { get; set; }
    }

    public interface IHaveSpice
    {
        ITinker Spice { get; }
    }

    // such tinker data is used just to hand out flavor
    public class FlavorTinkerData<T> : TinkerData, IHaveFlavor
        where T : Flavor
    {
        public T flavor;
        public Flavor Flavor
        {
            get => flavor;
            set => flavor = (T)value;
        }
    }
}