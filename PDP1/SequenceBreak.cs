using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDP1
{
    class SequenceBreak : IoDevice
    {
        public SequenceBreak(Cpu c) : base(c)
        {
        }

        public override void iot(uint inst)
        {
            UInt32 y;
            y = inst & 0x3f;
            if (y == 0x2d)                                                                                   // esm
            {
                cpu.SeqBreakFF = true;
                if (cpu.Debug) cpu.DebugLog += "esm        Entered sequence break mode";
            }
            else if (y == 0x2c)                                                                              // lsm
            {
                cpu.SeqBreakFF = false;
                if (cpu.Debug) cpu.DebugLog += "lsm        Leave sequence break mode";
            }
            else if (y == 0x2b)                                                                              // cac
            {
                cpu.BreakActive = 0;
                if (cpu.Debug) cpu.DebugLog += "cac        All sequence break channels off";
            }
            else if (y == 0x2e)                                                                              // cbs
            {
                cpu.BreakStarted = 0;
                cpu.BreakWaiting = 0;
                if (cpu.Debug) cpu.DebugLog += "cbs        Sequence break system cleared";
            }
            else if (y == 0x29)                                                                             // asc
            {
                y = (inst >> 6) & 0x0f;
                cpu.BreakActive |= (1 << (int)y);
                if (cpu.Debug) cpu.DebugLog += "asc        Sequence break channel " + y.ToString() + " activated";
            }
            else if (y == 0x028)                                                                           // dsc
            {
                y = (inst >> 6) & 0x0f;
                cpu.BreakActive |= (1 << (int)y);
                cpu.BreakActive ^= (1 << (int)y);
                if (cpu.Debug) cpu.DebugLog += "dsc        Sequence break channel " + y.ToString() + " deactivated";
            }
            else if (y == 0x02a)                                                                          // isb
            {
                y = (inst >> 6) & 0x0f;
                cpu.requestBreak((int)y);
                if (cpu.Debug) cpu.DebugLog += "isb        Sequence break channel " + y.ToString() + " initiated";
            }
        }
    }
}
