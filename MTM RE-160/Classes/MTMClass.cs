using System;

namespace MTM_RE_160.MTMConfig
{
    [Serializable]
   public  class MTMClass
    {
        public string ComPortName { get; set; }
        public string NameOfObject { get; set; }
        public string Address { get; set; }
        public string SpeedPort { get; set; }
        public string SpeedRegistration { get; set; }

        public override string ToString()
        {
            return Address;
        }
    }
}
