using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDP1
{
    class IoStatus : IoDevice
    {
        public IoStatus(Cpu c) : base(c)
        {
        }

        public override void iot(UInt32 inst)
        {
            UInt32 ioStatus;
            ioStatus = 0;
            if (cpu.TapeReader.CharacterReady) ioStatus |= 0x10000;
            if (cpu.TapePunch.Ready) ioStatus |= 0x02000;
            if (cpu.Terminal.KeyReady) ioStatus |= 0x04000;
            if (cpu.Terminal.Ready) ioStatus |= 0x8000;
            cpu.Io = ioStatus;
            if (cpu.Debug) cpu.DebugLog += "cks        IO = " + Form1.convert(cpu.Io, 18);
        }

    }
}
