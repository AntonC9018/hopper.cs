namespace Hopper.Utils.FS
{
    public class File : Node
    {
        public virtual File Copy()
        {
            return (File)this.MemberwiseClone();
        }
    }
}
