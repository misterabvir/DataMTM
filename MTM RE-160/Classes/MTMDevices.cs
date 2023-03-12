using MTM_RE_160.MTMConfig;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MTM_RE_160.Devices
{
    public class MTMDevices : CollectionBase, IList<MTMDevice>, IEnumerable
    {
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void AddMTMClasses(MTMClass mtmc)
        {
            byte address = Convert.ToByte(mtmc.Address);
            string portname = mtmc.ComPortName;
            DateTime dt = DateTime.Now;
            double hours = ((dt.Hour == 23.0) ? 24.0 * Settings.CountDay : dt.Hour + 1 + 24 * Settings.CountDay);
            double blocks = 60 / Convert.ToByte(mtmc.SpeedRegistration) * 60 * hours / 208 + 2;
            byte bl = blocks < 255 ? Convert.ToByte(blocks) : Convert.ToByte(255);
            int speed = Convert.ToInt32(mtmc.SpeedPort);

            MTMDevice mtm = new MTMDevice(address, portname, speed, bl, Settings.Path);

            List.Add(mtm);
        }


        public void Add(MTMDevice mtm)
        {
            List.Add(mtm);
        }

        public void Remove(MTMDevice mtm)
        {
            List.Remove(mtm);
        }

        public int IndexOf(MTMDevice item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, MTMDevice item)
        {
            List.Insert(index, item);
        }

        public bool Contains(MTMDevice item)
        {
            return List.Contains(item);
        }

        public void CopyTo(MTMDevice[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        bool ICollection<MTMDevice>.Remove(MTMDevice item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<MTMDevice> IEnumerable<MTMDevice>.GetEnumerator()
        {
            return List.GetEnumerator() as IEnumerator<MTMDevice>;
        }

        public MTMDevices()
        { }

        public MTMDevice this[int index]
        {
            get { return (MTMDevice)List[index]; }
            set { List[index] = value; }
        }

        internal void AddRange(MTMDevices mtms)
        {
            foreach (MTMDevice mtm in mtms)
            {
                List.Add(mtm);
            }
        }
        internal void AddRangeClasses(MTMs mtms)
        {
            foreach (MTMClass mtm in mtms)
            {
                this.AddMTMClasses(mtm);
            }
        }


    }
}
