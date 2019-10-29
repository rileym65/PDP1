using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDP1
{
    class IoDevice
    {
        protected Cpu cpu;

        public IoDevice(Cpu c)
        {
            cpu = c;
        }

        public virtual void iot(UInt32 inst)
        {
        }

        public virtual void HighSpeedCycle()
        {
        }

        public virtual void cycle()
        {
        }

    }
}
