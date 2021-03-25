namespace Hopper.Core.Components
{
    public struct Identifier 
    { 
        int number;
    }

    public struct Index<T> where T : IComponent
    {
        public Identifier componentId;
    }
}