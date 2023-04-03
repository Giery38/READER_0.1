using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Tools
{
    public class ObjectPool<T> where T : new()
    {
        private readonly ConcurrentBag<T> objects;
        private readonly Func<T> objectGenerator;

        public ObjectPool(Func<T> objectGenerator)
        {
            objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            objects = new ConcurrentBag<T>();
        }

        public T Get()
        {
            if (objects.TryTake(out T item))
            {
                return item;
            }
            return objectGenerator();
        }

        public void Put(T item)
        {
            objects.Add(item);
        }
    }
}
