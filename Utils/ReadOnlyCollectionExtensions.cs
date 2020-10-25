using System.Collections.ObjectModel;

namespace Core.Utils
{
    public static class ReadOnlyCollectionExtensions
    {
        public static T FindLast<T>(this ReadOnlyCollection<T> collection, System.Predicate<T> pred)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (pred(collection[i]))
                    return collection[i];
            }
            return default(T);
        }

        public static T Find<T>(this ReadOnlyCollection<T> collection, System.Predicate<T> pred)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (pred(collection[i]))
                    return collection[i];
            }
            return default(T);
        }
    }
}