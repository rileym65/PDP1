using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDP1
{
    class HighSpeedControl : IoDevice
    {
        private int irq;
        private UInt32 oneBuffer;
        private UInt32 twoBuffer;
        private UInt32 deviceRegister;
        private UInt32 initialRegister;
        private UInt32 wordCounter;
        private IoDevice[] devices;
        private IoDevice connectedDevice;

        public HighSpeedControl(Cpu c) : base(c)
        {
            irq = 1;
            wordCounter = 0;
            devices = new IoDevice[3];
            for (var i = 0; i < 3; i++) devices[i] = null;
            connectedDevice = null;
        }

        public override void HighSpeedCycle()
        {
            if ((deviceRegister & 0x4) == 0x4)
            {
                oneBuffer = cpu.Memory[initialRegister++];
                if (--wordCounter == 0)
                {
                    if (irq > 0) cpu.requestBreak(irq - 1);
                }
            }
            else
            {
                twoBuffer = oneBuffer;
                cpu.Memory[initialRegister++] = twoBuffer;
                if (--wordCounter == 0)
                {
                    if (irq > 0) cpu.requestBreak(irq - 1);
                }
            }
        }

        public override void iot(UInt32 inst)
        {
            UInt32 y;
            y = inst & 0x1ff;
            if (y == 0x26)                                                                                    // swc
            {
                wordCounter = cpu.Io & 0xffff;
                deviceRegister = (inst >> 9) & 0x7;
                connectedDevice = devices[deviceRegister & 3];
                if (cpu.Debug) cpu.DebugLog += "swc        Set word control: " + Form1.convert((UInt32)wordCounter, 8);
            }
            if (y == 0xe6)                                                                                   // sia
            {
                initialRegister = cpu.Io & 0xffff;
                if (cpu.Debug) cpu.DebugLog += "sia        Set initial address: " + Form1.convert((UInt32)initialRegister, 8);
            }
            if (y == 0x66)                                                                                   // sdf
            {
                deviceRegister = 0;
                if (cpu.Debug) cpu.DebugLog += "sdf        Stop data flow";
            }
        }

   }
}
