using System;
using System.Collections;
using System.Linq;

namespace OpenBank.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static void Apply<T>(this IEnumerable collection, Action<T> action) where T : class
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var casted in collection.OfType<T>())
            {
                action(casted);
            }
        }
    }
}