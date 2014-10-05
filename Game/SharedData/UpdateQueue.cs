using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedData
{
    class UpdateQueue
    {
        Queue<ISharedData> Queue = new Queue<ISharedData>();

        public void Enqueue(ISharedData item)
        {
            if (Queue.Contains(item))
                return;
            else
                Queue.Enqueue(item);
        }

        public ISharedData Dequeue()
        {
            if(Queue.Count>0)
            {
                return Queue.Dequeue();
            }
            return null; 
        }



    }
}
