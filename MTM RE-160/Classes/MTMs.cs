using System;
using System.Collections;
using System.Collections.Generic;

namespace MTM_RE_160.MTMConfig
{
    [Serializable]

    public class MTMs: CollectionBase, IList<MTMClass>, IEnumerable
    {
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(MTMClass mtm)
        {
            List.Add(mtm);
        }
        public void Remove(MTMClass mtm)
        {
            List.Remove(mtm);
        }

        public int IndexOf(MTMClass item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, MTMClass item)
        {
            List.Insert(index, item);
        }

        public bool Contains(MTMClass item)
        {
            return List.Contains(item);
        }

        public void CopyTo(MTMClass[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        bool ICollection<MTMClass>.Remove(MTMClass item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<MTMClass> IEnumerable<MTMClass>.GetEnumerator()
        {
            return List.GetEnumerator() as IEnumerator<MTMClass>;
        }

        public MTMs()
        { }

        public MTMClass this[int index]
        {
            get{ return (MTMClass)List[index]; }
            set { List[index] = value; }
        }

        internal void AddRange(MTMs mtms)
        {
            foreach (MTMClass mtm in mtms)
            {
                List.Add(mtm);
            }
        }
    }
}
