using System.Collections.Generic;

namespace Wrld.Materials
{
    class IdGenerator
    {
        public uint GetNextID()
        {
            uint id;

            lock (m_ids)
            {
                do
                {
                    id = ++m_idGenerator;
                }
                while (id == 0 || m_ids.Contains(id));

                m_ids.Add(id);
            }

            return id;
        }

        public void FreeID(uint id)
        {
            lock (m_ids)
            {
                m_ids.Remove(id);
            }
        }

        private uint m_idGenerator;
        private HashSet<uint> m_ids = new HashSet<uint>();
    };


}