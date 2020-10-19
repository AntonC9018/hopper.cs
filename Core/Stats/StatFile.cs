using Core.FS;
using System.Reflection;

namespace Core.Stats
{
    public class StatFile : File
    {
        public virtual void _Add(StatFile f, int sign)
        {
            // let's do it the dumbest way so that it works
            // maybe I'll figure out a better solution later
            var type = f.GetType();
            if (type != this.GetType())
            {
                throw new System.Exception("Can't add files of different types");
            }
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var oldVal = (int)field.GetValue(this);
                var addVal = (int)field.GetValue(f);
                var newVal = oldVal + sign * addVal;
                field.SetValue(this, newVal);
            }
        }
    }
}