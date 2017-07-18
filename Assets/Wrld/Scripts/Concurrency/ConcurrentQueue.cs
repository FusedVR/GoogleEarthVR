using System.Collections.Generic;

namespace Wrld.Concurrency
{
    // Here because Unity's target of Mono 2.0ish won't allow us to use the built in one.
    // This just wraps a queue with a lock. It won't be as good as the usual MS version 
    // (which tries to do low-lock/lock-free stuff), so this could do with revision if 
    // we see a lot of contention here.
    public class ConcurrentQueue<T>
    {
        private Queue<T> m_queue;

        public ConcurrentQueue()
        {
            m_queue = new Queue<T>();
        }
        public void Enqueue(T element)
        {
            lock (m_queue)
            {
                m_queue.Enqueue(element);
            }
        }

        public bool TryDequeue(out T element)
        {
            lock (m_queue)
            {
                if (m_queue.Count > 0)
                {
                    element = m_queue.Dequeue();

                    return true;
                }

                element = default(T);

                return false;
            }
        }
    }
}
