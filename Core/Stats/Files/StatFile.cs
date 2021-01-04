using Hopper.Utils.FS;
using System.Reflection;

namespace Hopper.Core.Stats
{
    public interface IAddableWith<in T> where T : IAddableWith<T>
    {
        void _Add(T f, int sign);
    }

    public class StatFile : File, IAddableWith<StatFile>
    {
        public void _Add(StatFile f, int sign)
        {
            // let's do it the dumbest way so that it works
            // maybe I'll figure out a better solution later
            var type = f.GetType();
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