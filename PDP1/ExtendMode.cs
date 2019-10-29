using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDP1
{
    class ExtendMode : IoDevice
    {
        public ExtendMode(Cpu c) : base(c)
        {
        }

        public override void iot(UInt32 inst)
        {
            if ((inst & 0x800) == 0x800)                                                   // eem
            {
                cpu.Extend = true;
                if (cpu.Debug) cpu.DebugLog += "eem        Entered extend mode";
            }
            else                                                                           // lem
            {
                cpu.Extend = false;
                if (cpu.Debug) cpu.DebugLog += "lem        Leave extend mode";
            }
        }

    }
}
