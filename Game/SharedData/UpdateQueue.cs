using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedData
{
    class UpdateQueue
    {
        Queue<TypeList.ItemUpdate> Queue = new Queue<TypeList.ItemUpdate>();

        public void Enqueue(TypeList.ItemUpdate item)
        {
            lock (Queue)
            {
                if (Queue.FirstOrDefault((o) => o.Item == item.Item && item.Change == o.Change) != null)
                    return;
                else
                    Queue.Enqueue(item);
            }
        }

        public TypeList.ItemUpdate Dequeue()
        {
            lock (Queue)
            {
                if (Queue.Count > 0)
                {
                    return Queue.Dequeue();
                }
                return default(TypeList.ItemUpdate);
            }
        }



    }
}
