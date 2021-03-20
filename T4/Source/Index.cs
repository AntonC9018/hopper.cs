namespace Test.Stats
{
    public struct ID { public int id; }
    
    public interface IStat{}

    public class Index<T> where T : IStat 
    {
        public ID id;
    }
}