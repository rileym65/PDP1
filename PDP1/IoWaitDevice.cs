using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDP1
{
    class IoWaitDevice : IoDevice
    {
        public IoWaitDevice(Cpu c) : base(c)
        {
        }

        public override void iot(UInt32 inst)
        {
            Boolean wait;
            Boolean pulse;
            wait = ((inst & 0x1000) == 0x1000);
            pulse = ((inst & 0x0800) == 0x0800);
            pulse ^= wait;
            cpu.IoHaltFF = wait;
            if (cpu.Debug) cpu.DebugLog += "iot        ";
        }
    }
}
