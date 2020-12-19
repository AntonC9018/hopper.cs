namespace Hopper.Core.Behaviors
{
    public interface IInitable<Config>
    {
        void Init(Config config);
    }

    public interface IInitable
    {
        void Init();
    }
}