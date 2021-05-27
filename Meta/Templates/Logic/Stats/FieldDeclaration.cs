namespace Hopper.Meta.Stats
{
    public struct FieldDeclaration
    {
        public string name;
        public FieldMetadata metadata;

        public FieldDeclaration(string name, FieldMetadata metadata)
        {
            this.name = name;
            this.metadata = metadata;
        }
    }

    public struct StaticObjectFieldDeclaration
    {
        public string name;
        public StaticStatFieldMetadata metadata;

        public StaticObjectFieldDeclaration(string name, StaticStatFieldMetadata metadata)
        {
            this.name = name;
            this.metadata = metadata;
        }
    }
}