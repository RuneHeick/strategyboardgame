using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedData
{
    class UpdateQueue<T>
    {
        Queue<T> Queue = new Queue<T>();

        public void Enqueue(T item)
        {
            if (Queue.Contains(item))
                return;
            else
                Queue.Enqueue(item);
        }

        public T Dequeue()
        {
            if(Queue.Count>0)
            {
                return Queue.Dequeue();
            }
            return default(T); 
        }



    }
}
